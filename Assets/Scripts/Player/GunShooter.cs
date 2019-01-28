using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GunShooter : MonoBehaviour
{
	const float SHOT_INTERVAL = 0.1f;
	const float MAX_RAY_DISTANCE = 100f;

	[SerializeField] BulletPool _bulletPool;
	[SerializeField] GameObject _hitTarget;
	[SerializeField] GameObject _ricochetFirePrefab;
	[SerializeField] ParticleSystem _gunFireParticle;

	Ray _ray;
	RaycastHit _hitInfo;
	bool _isHit;
	[SerializeField] AudioClip _shotSound;
	float _timeAfterShot;　
	bool _canShoot;

	// Use this for initialization
	void Start ()
	{
		_canShoot = true;
	}

	// Update is called once per frame
	void Update ()
	{
		_ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		_isHit = Physics.Raycast (_ray, out _hitInfo, MAX_RAY_DISTANCE);

		TurnGun ();
		Shoot ();
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

	void Shoot ()
	{
		if (Input.GetMouseButtonDown (0) && _canShoot)
		{
			// shot interval
			_canShoot = false;
			Observable.Timer (TimeSpan.FromSeconds (SHOT_INTERVAL))
				.Subscribe (_ => _canShoot = true);
			// 銃弾
			_bulletPool.ShootBullet ();
			// 火花パーティクル
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
					GameObject _ricochetFire = Instantiate (_ricochetFirePrefab, _hitInfo.point, Quaternion.identity);
					_ricochetFire.GetComponent<ParticleSystem> ().Play ();
					AudioSource.PlayClipAtPoint (_shotSound, _ray.GetPoint (_hitInfo.distance));
					Destroy (_ricochetFire, 0.1f);
				}
			}
		}
	}
}