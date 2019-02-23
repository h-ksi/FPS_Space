using System;
using UnityEngine;

public class BulletPool : ObjectPool<Bullet>
{
  public Action<Bullet> ExtraOnCreateInstance,
  ExtraOnPull,
  ExtraOnReturn;

  public BulletPool(Bullet poolObject, int initialPoolSize = 1, int maxPoolSize = int.MaxValue, Transform root = null, float timeUntilReturn = 0, int remainCount = 0) : base(poolObject, initialPoolSize, maxPoolSize, root, timeUntilReturn, remainCount)
  {
    this.root = new GameObject("Bullets").transform;
  }

  protected override void OnCreateInstance(Bullet instance)
  {
    if (ExtraOnCreateInstance != null)
      ExtraOnCreateInstance(instance);
  }

  protected override void OnPull(Bullet instance)
  {
    instance.gameObject.SetActive(true);

    if (ExtraOnPull != null)
      ExtraOnPull(instance);
  }

  protected override void OnReturn(Bullet instance)
  {
    instance.gameObject.SetActive(false);

    if (ExtraOnReturn != null)
      ExtraOnReturn(instance);
  }
}