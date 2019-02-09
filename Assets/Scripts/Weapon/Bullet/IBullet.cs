using UnityEngine;

public interface IBullet
{
	void SetMuzzleTransform (Transform muzzle);
	void Shoot (float shotSpeed);
	void IgniteRicochetFire (Vector3 hitPosition);
}