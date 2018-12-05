using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Ext
{
    // 水平面での方向ベクトルを取得する
    public static Vector3 GetHorizontalDirection(this Vector3 originalVector)
    {
        return new Vector3(originalVector.x, 0, originalVector.z).normalized;
    }
}
