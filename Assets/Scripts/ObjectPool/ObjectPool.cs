using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UniRx;

public class ObjectPool<T> : IDisposable where T : UnityEngine.Component
{
  Queue<T> pool;
  Stack<T> trash;
  List<T> returnTimerList;

  bool isDisposed = false;

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
    get { return pool.Count; }
  }

  /// <summary>
  /// Current trashed object count.
  /// </summary>
  public int TrashCount
  {
    get { return trash.Count; }
  }

  /// <summary>
  /// Limit of pooling size.
  /// </summary>
  public int MaxPoolSize
  {
    get { return maxPoolSize; }
  }

  /// <summary>
  /// Destructor.
  /// </summary>
  ~ObjectPool()
  {
    if (!isDisposed)
    {
      Dispose(false);

      isDisposed = true;
    }
  }

  /// <summary>
  /// Constructor.
  /// </summary>
  public ObjectPool(T poolObject, int initialPoolSize = 1, int maxPoolSize = int.MaxValue, Transform root = null, float timeUntilReturn = 0, int remainCount = 0)
  {
    if (initialPoolSize < 1)
      throw new ArgumentOutOfRangeException("initialPoolSize must be larger than 0");
    if (maxPoolSize < initialPoolSize)
      throw new ArgumentOutOfRangeException("maxPoolSize must be larger than initialPoolSize");

    this.poolObject = poolObject;
    this.initialPoolSize = initialPoolSize;
    this.maxPoolSize = maxPoolSize;
    this.root = root;
    this.timeUntilReturn = timeUntilReturn;
    this.remainCount = remainCount;

    pool = new Queue<T>();
    trash = new Stack<T>();
    returnTimerList = new List<T>();

    LoadPerFrame(initialPoolSize);
  }

  /// <summary>
  /// Get instance from pool and add it to trash.
  /// </summary>
  public T Pull()
  {
    if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
    if (pool == null) pool = new Queue<T>();
    if (trash == null) trash = new Stack<T>();
    if (returnTimerList == null) returnTimerList = new List<T>();

    T obj;
    // In the case that TrashCount reaches the upper limit
    if (TrashCount == maxPoolSize)
    {
      // Reuse an object from trash
      obj = trash.Pop();
    }
    // In the case that pool is empty
    else if (PoolCount == 0)
    {
      obj = CreateInstance();
    }
    else
    {
      obj = pool.Dequeue();
    }

    OnPull(obj);
    trash.Push(obj);

    if (returnTimerList.Count < TrashCount - remainCount)
    {
      ReturnTimer(obj, timeUntilReturn);
    }

    return obj;
  }

  /// <summary>
  /// Return instance to pool and discard instance from trash.
  /// </summary>
  public void Return(T instance)
  {
    if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
    if (pool == null) pool = new Queue<T>();
    if (trash == null) trash = new Stack<T>();

    OnReturn(instance);
    pool.Enqueue(instance);

    // Discard an instance from trash
    if (TrashCount > 0) trash.Pop();
  }

  /// <summary>
  /// Create instance of T object.
  /// </summary>
  protected virtual T CreateInstance()
  {
    if (root == null)
    {
      GameObject clonesRoot = new GameObject("Clones");
      root = clonesRoot.transform;
    }

    T newObject = Object.Instantiate(poolObject as Object, root) as T;
    OnCreateInstance(newObject);
    return newObject;
  }

  /// <summary>
  /// Called on Pull().
  /// </summary>
  protected virtual void OnPull(T instance)
  {
    instance.gameObject.SetActive(true);
  }

  /// <summary>
  /// Called on Return().
  /// </summary>
  protected virtual void OnReturn(T instance)
  {
    instance.gameObject.SetActive(false);
  }

  /// <summary>
  /// Called after Instantiate().
  /// </summary>
  protected virtual void OnCreateInstance(T instance)
  {

  }

  /// <summary>
  /// Called on Clear().
  /// </summary>
  protected virtual void OnClear(T instance)
  {
    if (instance == null) return;

    var go = instance.gameObject;
    if (go == null) return;
    Object.Destroy(go);
  }

  /// <summary>
  /// Withdraw multiple items.
  /// </summary>	
  public T[] Pull(int count)
  {
    if (count <= 0)
      return new T[0];

    T[] group = new T[count];
    for (int i = 0; i < count; i++)
    {
      group[i] = Pull();
    }
    return group;
  }

  /// <summary>
  /// Return multiple items.
  /// </summary>	
  public void Return(IEnumerable<T> objects)
  {
    foreach (T obj in objects) { Return(obj); }
  }

  /// <summary>
  /// Clear collection items.
  /// </summary>
  public void Clear()
  {
    foreach (T item in pool)
    {
      OnClear(item);
      Object.Destroy(item as Object);
    }
    pool.Clear();
    trash.Clear();
    returnTimerList.Clear();
  }

  /// <summary>
  /// Return instance after specified seconds.
  /// </summary>
  public void ReturnTimer(T instance, float sec)
  {
    if (sec <= 0) return;
    if (returnTimerList == null)
      returnTimerList = new List<T>();

    if (returnTimerList.Count < maxPoolSize)
    {
      Observable.Timer(TimeSpan.FromSeconds(sec))
        .Subscribe(_ =>
       {
         Return(instance);
         returnTimerList.Remove(instance);
       }).AddTo(instance);

      returnTimerList.Add(instance);
    }
  }

  /// <summary>
  ///  Load pool objects every frame until it reaches a specific amount.
  /// </summary>
  /// <param name="loadCount">Required count of instances.</param>
  /// <param name="countPerFrame">Count of generated instances per frame.</param>
  public void LoadPerFrame(int loadCount, int countPerFrame = 1)
  {
    Observable.FromMicroCoroutine(_ => LoadPerFrameCore(loadCount, countPerFrame)).Subscribe();
  }

  IEnumerator LoadPerFrameCore(int loadCount, int countPerFrame)
  {
    while (PoolCount < loadCount)
    {
      int requireCount = loadCount - PoolCount;
      if (requireCount <= 0) break;

      int createCount = Math.Min(requireCount, countPerFrame);

      for (int i = 0; i < createCount; i++)
      {
        T instance = CreateInstance();
        Return(instance);
      }

      yield return null;
    }
  }

  #region Dispose

  protected virtual void Dispose(bool isDisposing)
  {
    if (isDisposed) return;

    if (isDisposing) Clear();

    // Set large field members null.
    pool = null;
    trash = null;
    returnTimerList = null;

    isDisposed = true;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  #endregion Dispose
}