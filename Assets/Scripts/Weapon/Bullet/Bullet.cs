using UnityEngine;

public class Bullet : MonoBehaviour, IBullet
{
  [SerializeField] Rigidbody _rb;
  [SerializeField] ParticleSystem _ricochetFire;
  [SerializeField] AudioClip _shotSound;
  Transform _muzzle;
  ParticleSystem.EmitParams emitParams;

  void Awake()
  {
    emitParams = new ParticleSystem.EmitParams();
    emitParams.applyShapeToPosition = true;
  }

  public void Shoot(float shotSpeed)
  {
    transform.position = _muzzle.position;
    transform.rotation = _muzzle.rotation;
    transform.Rotate(_muzzle.right, 90f);
    _rb.velocity = shotSpeed * _muzzle.forward;
  }

  public void SetMuzzleTransform(Transform muzzle)
  {
    _muzzle = muzzle;
  }

  public void IgniteRicochetFire(Vector3 hitPosition)
  {
    AudioSource.PlayClipAtPoint(_shotSound, hitPosition);

    if (!_ricochetFire.isPlaying)
    {
      emitParams.position = hitPosition;

      // ParticleSystem.Emit()を使用する際、
      // パーティクルがオブジェクトに追随しないようにするにはInspector上で
      // PartycleSystemのSimulationSpaceを"World"に設定する必要がある
      _ricochetFire.Emit(emitParams, 1);
    }
  }
}