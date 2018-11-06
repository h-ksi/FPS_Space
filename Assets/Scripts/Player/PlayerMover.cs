using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMover : MonoBehaviour 
	{
		[SerializeField]private CharacterController characterController;
		private Vector3 velocityVector = Vector3.zero;
		private const float JUMP_SPEED = 4f;
		private float gravity = Physics.gravity.y;	//	-9.81;

		public void MovePlayer(
			Transform fpsCameraTransform,
			float moveV, 
			float moveH, 
			float speed, 
			bool isGrounded, 
			bool jumpKey
		)
		{		
			// FPSCamera単位方向ベクトル
			Vector3 fpsCameraDirection = fpsCameraTransform.forward * moveV + fpsCameraTransform.right * moveH;
			if(fpsCameraDirection != Vector3.zero)
			{
				fpsCameraDirection.Normalize();
			}

			// 速度ベクトルの方向をFPSCamera方向に設定
			velocityVector.x = fpsCameraDirection.x * speed;
			velocityVector.z = fpsCameraDirection.z * speed;

			// 接地時
			if (isGrounded)
			{
				// Jump
				if(jumpKey)
				{
					velocityVector.y = JUMP_SPEED;
				}
			}
			// 非接地時
			else
			{
				// 重力による自由落下処理
				velocityVector.y += gravity * Time.fixedDeltaTime;
			}
			
			characterController.Move(velocityVector * Time.fixedDeltaTime);
		}
	}
}