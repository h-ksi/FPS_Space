// 参考元
// https://qiita.com/toRisouP/items/9141f1bbc6f623db5fdd
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CheckGroundedWithRaycast : MonoBehaviour
{	
	private int layer_mask;
	private Ray ray;
	private float tolerance = 1.3f;
	private bool isGroundedWithRaycast;

	[SerializeField]private CharacterController characterController;

	public void Start()
	{
		layer_mask = LayerMask.GetMask("Ground");
	}

	public bool CheckPlayerIsGrounded()
	{
		//CharacterControlle.IsGroundedがtrueならRaycastを使わずに判定終了
		if (characterController.isGrounded)
		{
			// Debug.Log("CharacterController detected ground");
			return true;
		}
		
		//放つ光線の初期位置と姿勢
		//若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
		ray = new Ray(this.transform.position + Vector3.up * 0.01f, Vector3.down);
		
		//Raycastがhitするかどうかで判定
		//地面にのみ衝突するようにレイヤを指定する
		isGroundedWithRaycast = Physics.Raycast(ray, tolerance, layer_mask);
		
		return isGroundedWithRaycast;
	}
}
