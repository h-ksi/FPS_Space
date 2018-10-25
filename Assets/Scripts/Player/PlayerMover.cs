using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMover : MonoBehaviour 
	{
		private CharacterController charaController;
		private Vector3 velocityVector = Vector3.zero;

		[Range(1f, 15f)]
		private float jumpSpeed = 4f;
		private float gravity;

		public void Start()
		{
			gravity = Physics.gravity.y;	//	-9.81	
			charaController = GetComponent<CharacterController>();
		}

		public void MovePlayer
		(
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
					velocityVector.y = jumpSpeed;
				}
			}
			// 非接地時
			else
			{
				// 重力による自由落下処理
				velocityVector.y += gravity * Time.fixedDeltaTime;
			}
			
			charaController.Move(velocityVector * Time.fixedDeltaTime);
		}
	}
}