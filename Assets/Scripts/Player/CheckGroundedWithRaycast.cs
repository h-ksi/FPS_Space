// 参考元
// https://qiita.com/toRisouP/items/9141f1bbc6f623db5fdd
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(CharacterController))]
public class CheckGroundedWithRaycast : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayerMask;
    private Ray _ray;
    private float _tolerance = 1.3f;
    private bool _isGroundedWithRaycast;
    [SerializeField] private CharacterController _characterController;

    public bool CheckPlayerIsGrounded()
    {
        //CharacterControlle.IsGroundedがtrueならRaycastを使わずに判定終了
        if (_characterController.isGrounded)
        {
            // Debug.Log("_characterController detected ground");
            return true;
        }

        //放つ光線の初期位置と姿勢
        //若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
        _ray = new Ray(this.transform.position + Vector3.up * 0.01f, Vector3.down);

        //Raycastがhitするかどうかで判定
        //地面にのみ衝突するようにレイヤを指定する
        _isGroundedWithRaycast = Physics.Raycast(_ray, _tolerance, _groundLayerMask.value);

        return _isGroundedWithRaycast;
    }
}
