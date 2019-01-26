using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class GunShooter : MonoBehaviour
{
	const float MAX_RAY_DISTANCE = 100f;

	[SerializeField] float _shotSpeed = 100f;
	[SerializeField] float _shotInterval = 0.1f;
	[SerializeField] int _pooledAmount = 10;
	[SerializeField] Transform _bulletsRoot;
	[SerializeField] GameObject _bulletPrefab;
	[SerializeField] ParticleSystem _gunFireParticle;
	[SerializeField] GameObject _ricochetFirePrefab;
	[SerializeField] AudioClip _shotSound;
	[SerializeField] GameObject _hitTarget;
	[SerializeField] float _timeUntilBulletIsUnable = 6f;

	SimplePool<Rigidbody> _bulletsRbPool;
	SimplePool<ParticleSystem> _ricochetParticlePool;
	string _bulletRbPoolName = "BulletRigidbodyPool";
	string _ricochetParticlePoolName = "RicochetPatriclePool";

	Ray _ray;
	RaycastHit _hitInfo;
	bool _isHit;
	float _timeAfterShot;

	public ReactiveProperty<bool> IsClicked { get; private set; }

	void Awake ()
	{
		IsClicked = new ReactiveProperty<bool> (false);

		PrepareRigidbodyPool ();
		PrepareRicochetParticlePool ();

		// GetPool後に非表示にするまでの時間を設定
		SimplePoolHelper.GetPool<WaitForSeconds> ().CreateFunction = (template) => new WaitForSeconds (_timeUntilBulletIsUnable);
	}

	void Start ()
	{
		this.UpdateAsObservable ()
			.Subscribe (_ =>
			{
				IsClicked.Value = Input.GetMouseButtonDown (0);
				_ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				_isHit = Physics.Raycast (_ray, out _hitInfo, MAX_RAY_DISTANCE);

				TurnGun ();
			});

		IsClicked.Where (x => x)
			.ThrottleFirst (TimeSpan.FromSeconds (_shotInterval))
			.Subscribe (_ => Shoot ());
	}

	void Shoot ()
	{
		// 銃弾をObjectPoolから取り出す
		SimplePoolHelper.Pop<Rigidbody> (_bulletRbPoolName);
		// 火花パーティクルの再生
		_gunFireParticle.Play ();
		// 銃声
		AudioSource.PlayClipAtPoint (_shotSound, transform.position);

		if (_isHit)
		{
			if (_hitInfo.transform.tag == _hitTarget.name)
			{
				//	ヒット時の処理;
				Debug.Log ("hit " + _hitTarget.name);
			}
			else
			{
				// 跳弾による火花エフェクト
				SimplePoolHelper.Pop<ParticleSystem> (_ricochetParticlePoolName);
				AudioSource.PlayClipAtPoint (_shotSound, _ray.GetPoint (_hitInfo.distance));
			}
		}
	}

	void TurnGun ()
	{
		if (_isHit)
		{
			transform.parent.LookAt (_hitInfo.point);
			transform.parent.Rotate (4, 0, 0);
			Debug.DrawLine (transform.position, _hitInfo.point, Color.red);
		}
		else
		{
			Vector3 vec = _ray.GetPoint (MAX_RAY_DISTANCE) - transform.position;
			transform.parent.forward = vec.normalized;
			Debug.DrawLine (transform.position, _ray.GetPoint (MAX_RAY_DISTANCE), Color.red);
		}
	}

	void PrepareRigidbodyPool ()
	{
		_bulletsRbPool = SimplePoolHelper.GetPool<Rigidbody> (_bulletRbPoolName);
		_bulletsRbPool.OnPush = (item) => item.gameObject.SetActive (false);
		_bulletsRbPool.OnPop = (item) =>
		{
			if (item.transform.parent != _bulletsRoot)
			{
				item.transform.parent = _bulletsRoot;
			}

			item.position = transform.position;
			item.gameObject.SetActive (true);
			item.velocity = _shotSpeed * transform.forward;

			// 6秒後に弾をPoolに返却する
			Observable.Timer (TimeSpan.FromSeconds (6f))
				.Subscribe (_ => SimplePoolHelper.Push (item, _bulletRbPoolName));
		};
		_bulletsRbPool.CreateFunction = (template) =>
		{
			GameObject newBullet = Instantiate (_bulletPrefab, transform.position, Quaternion.identity);
			if (_bulletsRoot == null)
			{
				GameObject bulletsRoot = new GameObject ("Bullets");
				_bulletsRoot = bulletsRoot.transform;
			}
			newBullet.transform.parent = _bulletsRoot;
			Vector3 rot = transform.localEulerAngles;
			rot.x += 90f;
			newBullet.transform.rotation = Quaternion.Euler (rot);
			Rigidbody _rb = newBullet.GetComponent<Rigidbody> ();
			return _rb;
		};
		_bulletsRbPool.Populate (_pooledAmount);
	}

	void PrepareRicochetParticlePool ()
	{
		_ricochetParticlePool = SimplePoolHelper.GetPool<ParticleSystem> (_ricochetParticlePoolName);
		_ricochetParticlePool.OnPush = (item) => item.gameObject.SetActive (false);
		_ricochetParticlePool.OnPop = (item) =>
		{
			if (item.transform.parent != _bulletsRoot)
			{
				item.transform.parent = _bulletsRoot;
			}

			item.gameObject.SetActive (true);
			item.Play ();

			// 0.1秒後にParticleSystemをPoolに返却
			Observable.Timer (TimeSpan.FromSeconds (0.1f)).Subscribe (_ =>
			{
				SimplePoolHelper.Push (item, _ricochetParticlePoolName);
			}).AddTo (this);
		};
		_ricochetParticlePool.CreateFunction = (template) =>
		{
			GameObject newRicochetParticle = Instantiate (_ricochetFirePrefab, _hitInfo.point, Quaternion.identity);
			if (_bulletsRoot == null)
			{
				GameObject bulletsRoot = new GameObject ("Bullets");
				_bulletsRoot = bulletsRoot.transform;
			}
			newRicochetParticle.transform.parent = _bulletsRoot;
			ParticleSystem _ricochetPs = newRicochetParticle.GetComponent<ParticleSystem> ();
			return _ricochetPs;
		};
	}

	// Called when the object is destroyed
	void OnDestroyRigidbodyPoolObject ()
	{
		SimplePoolHelper.GetPool<Rigidbody> (_bulletRbPoolName).OnPop = (item) => item.gameObject.SetActive (true);
		SimplePoolHelper.GetPool<Rigidbody> (_bulletRbPoolName).Clear ();
	}

	void OnDestroyRicochetParticlePoolObject ()
	{
		SimplePoolHelper.GetPool<ParticleSystem> (_ricochetParticlePoolName).OnPop = (item) => item.gameObject.SetActive (true);
		SimplePoolHelper.GetPool<ParticleSystem> (_ricochetParticlePoolName).Clear ();
	}
}