using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class GunShooter : MonoBehaviour
{
	const float MAX_RAY_DISTANCE = 100f;
	[SerializeField] float _shotSpeed = 100f;
	[SerializeField] float _shotInterval = 0.1f;
	[SerializeField] ParticleSystem _gunFireParticle;
	[SerializeField] Bullet _bullet;
	[SerializeField] Transform _hitTarget;
	[SerializeField] AudioClip _shotSound;
	[SerializeField] Transform _bulletsRoot;
	[SerializeField] int _initialBulletPoolSize;
	[SerializeField] int _maxBulletPoolSize;
	[SerializeField] float _deactivationTime;

	BulletPool _bulletPool;
	Rigidbody _rb;
	Ray _ray;
	RaycastHit _hitInfo;
	bool _isHit;

	public ReactiveProperty<bool> IsClicked { get; private set; }

	void Awake ()
	{
		IsClicked = new ReactiveProperty<bool> (false);
		PrepareBulletPool ();
	}

	void Start ()
	{
		this.UpdateAsObservable ().Subscribe (_ =>
		{
			_ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			_isHit = Physics.Raycast (_ray, out _hitInfo, MAX_RAY_DISTANCE);
			IsClicked.Value = Input.GetMouseButtonDown (0);
			TurnGun ();
		});

		IsClicked.Where (x => x)
			.ThrottleFirst (TimeSpan.FromSeconds (_shotInterval))
			.Subscribe (_ => Shoot ());
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
			Vector3 gunDirection = _ray.GetPoint (MAX_RAY_DISTANCE) - transform.position;
			transform.parent.forward = gunDirection.normalized;
			Debug.DrawLine (transform.position, _ray.GetPoint (MAX_RAY_DISTANCE), Color.red);
		}
	}

	void Shoot ()
	{
		// 銃弾
		_bulletPool.Pull ();

		// 火花パーティクル
		_gunFireParticle.Play ();
		// 銃声
		AudioSource.PlayClipAtPoint (_shotSound, transform.position);

		if (_isHit)
		{
			if (_hitInfo.transform.tag == _hitTarget.name)
			{
				// ヒット時の処理;
				Debug.Log ("hit " + _hitTarget.name);
			}
		}
	}

	void PrepareBulletPool ()
	{
		_bulletPool = new BulletPool (_bullet, _initialBulletPoolSize, _maxBulletPoolSize, timeUntilReturn : _deactivationTime);

		_bulletPool.ExtraOnCreateInstance = (bullet) =>
		{
			bullet.SetMuzzleTransform (_bulletsRoot);
		};

		_bulletPool.ExtraOnPull = (bullet) =>
		{
			bullet.Shoot (_shotSpeed);

			if (_isHit && _hitInfo.transform.tag != _hitTarget.name)
			{
				// 跳弾火花パーティクル
				bullet.IgniteRicochetFire (_ray.GetPoint (_hitInfo.distance));
			}
		};
	}
}