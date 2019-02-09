using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using UniRx;

public class GenericObjectPool<T> : IDisposable where T : UnityEngine.Component
{
	bool isDisposed = false;

	Queue<T> pool;
	Stack<T> trash;
	List<T> returnTimerList;

	protected T poolObject;
	protected int initialPoolSize;
	protected int maxPoolSize;
	protected Transform root;
	protected float timeUntilReturn;
	protected int remainCount;

	/// <summary>
	/// Current pooled object count.
	/// </summary>
	public int PoolCount
	{
		get
		{
			if (pool == null) return 0;
			return pool.Count;
		}
	}

	/// <summary>
	/// Current trashed object count.
	/// </summary>
	public int TrashCount
	{
		get
		{
			if (trash == null) return 0;
			return trash.Count;
		}
	}

	/// <summary>
	/// Limit of pooling size.
	/// </summary>
	public int MaxPoolSize
	{
		get
		{
			return maxPoolSize;
		}
	}

	/// <summary>
	/// Destructor.
	/// </summary>
	~GenericObjectPool ()
	{
		Dispose (false);
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public GenericObjectPool (T poolObject, int initialPoolSize = 1, int maxPoolSize = int.MaxValue, Transform root = null, float timeUntilReturn = 0, int remainCount = 0)
	{
		if (initialPoolSize < 1)
			throw new ArgumentOutOfRangeException ("initialPoolSize must be larger than 0");
		if (maxPoolSize < initialPoolSize)
			throw new ArgumentOutOfRangeException ("maxPoolSize must be larger than initialPoolSize");

		this.poolObject = poolObject;
		this.initialPoolSize = initialPoolSize;
		this.maxPoolSize = maxPoolSize;
		this.root = root;
		this.timeUntilReturn = timeUntilReturn;
		this.remainCount = remainCount;

		pool = new Queue<T> ();
		trash = new Stack<T> ();
		returnTimerList = new List<T> ();

		PreloadPerFrame (initialPoolSize).Subscribe ();
	}

	/// <summary>
	/// Get instance from pool and add it to trash.
	/// </summary>
	public T Pull ()
	{
		if (isDisposed) throw new ObjectDisposedException ("ObjectPool was already disposed.");
		if (trash == null) trash = new Stack<T> ();

		T obj;
		// In the case that TrashCount reaches the upper limit
		if (TrashCount == maxPoolSize)
		{
			// Reuse an object from trash
			obj = trash.Pop ();
		}
		// In the case that pool is empty
		else if (PoolCount == 0)
		{
			obj = CreateInstance ();
		}
		else
		{
			obj = pool.Dequeue ();
		}

		OnPull (obj);
		trash.Push (obj);

		if (returnTimerList.Count < TrashCount - remainCount)
		{
			ReturnTimer (obj, timeUntilReturn);
		}

		return obj;
	}

	/// <summary>
	/// Return instance to pool and discard instance from trash.
	/// </summary>
	public void Return (T instance)
	{
		if (isDisposed) throw new ObjectDisposedException ("ObjectPool was already disposed.");
		if (pool == null) pool = new Queue<T> ();
		if (trash == null) trash = new Stack<T> ();

		OnReturn (instance);
		pool.Enqueue (instance);

		// Discard an instance from trash
		if (TrashCount > 0) trash.Pop ();
	}

	/// <summary>
	/// Create instance of T object.
	/// </summary>
	protected T CreateInstance ()
	{
		T newObject = Object.Instantiate (poolObject as Object) as T;
		OnCreateInstance (newObject);
		return newObject;
	}

	/// <summary>
	/// Called on Pull().
	/// </summary>
	protected virtual void OnPull (T instance)
	{
		instance.gameObject.SetActive (true);
	}

	/// <summary>
	/// Called on Return().
	/// </summary>
	protected virtual void OnReturn (T instance)
	{
		instance.gameObject.SetActive (false);
	}

	/// <summary>
	/// Called after Instantiate().
	/// </summary>
	protected virtual void OnCreateInstance (T instance)
	{
		if (root == null)
		{
			GameObject bulletsRoot = new GameObject ("Clones");
			root = bulletsRoot.transform;
		}

		instance.gameObject.transform.SetParent (root);
	}

	/// <summary>
	/// Called on Clear().
	/// </summary>
	protected virtual void OnClear (T instance)
	{
		if (instance == null) return;

		var go = instance.gameObject;
		if (go == null) return;
		Object.Destroy (go);
	}

	/// <summary>
	/// Withdraw multiple items.
	/// </summary>	
	public T[] Pull (int count)
	{
		if (count <= 0)
			return new T[0];

		T[] group = new T[count];
		for (int i = 0; i < count; i++)
		{
			group[i] = Pull ();
		}
		return group;
	}

	/// <summary>
	/// Return multiple items.
	/// </summary>	
	public void Return (IEnumerable<T> objects)
	{
		foreach (T obj in objects) { Return (obj); }
	}

	/// <summary>
	/// Called when clear or disposed, useful for destroy instance or other finalize method.
	/// </summary>
	public void Clear ()
	{
		foreach (T item in pool)
		{
			OnClear (item);
			Object.Destroy (item as Object);
		}
		pool.Clear ();
		trash.Clear ();
		returnTimerList.Clear ();
	}

	/// <summary>
	/// Return instance after specified seconds.
	/// </summary>
	public void ReturnTimer (T instance, float sec)
	{
		if (sec <= 0) return;
		if (returnTimerList == null)
			returnTimerList = new List<T> ();

		if (returnTimerList.Count < maxPoolSize)
		{
			Observable.Timer (TimeSpan.FromSeconds (sec))
				.Subscribe (_ =>
				{
					Return (instance);
					returnTimerList.Remove (instance);
				}).AddTo (instance);

			returnTimerList.Add (instance);
		}
	}

	/// <summary>
	///  Prepare pool in advance.
	/// </summary>
	/// <param name="preloadCount">Count of generated instances in the end.</param>
	/// <param name="countPerFrame">Count of generated instances per frame.</param>
	public IObservable<Unit> PreloadPerFrame (int preloadCount, int countPerFrame = 1)
	{
		if (pool == null)
			pool = new Queue<T> (preloadCount);

		return Observable.FromMicroCoroutine<Unit> ((observer, cancel) => PreloadCore (preloadCount, countPerFrame, observer, cancel));
	}

	IEnumerator PreloadCore (int preloadCount, int countPerFrame, IObserver<Unit> observer, CancellationToken cancellationToken)
	{
		while (PoolCount < preloadCount && !cancellationToken.IsCancellationRequested)
		{
			int requireCount = preloadCount - PoolCount;
			if (requireCount <= 0) break;

			int createCount = Math.Min (requireCount, countPerFrame);

			for (int i = 0; i < createCount; i++)
			{
				try
				{
					var instance = CreateInstance ();
					Return (instance);
				}
				catch (Exception ex)
				{
					observer.OnError (ex);
					yield break;
				}
			}

			yield return null;
		}

		observer.OnNext (Unit.Default);
		observer.OnCompleted ();
	}

	#region IDisposable Support

	protected virtual void Dispose (bool isDisposing)
	{
		if (!isDisposed)
		{
			if (isDisposing)
			{
				Clear ();
			}

			// Set large field members null.
			pool = null;
			trash = null;
			returnTimerList = null;

			isDisposed = true;
		}
	}

	public void Dispose ()
	{
		Dispose (true);
		GC.SuppressFinalize (this);
	}

	#endregion IDisposable Support
}