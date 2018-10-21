using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	[RequireComponent(typeof(CharacterController),typeof(CheckGroundedWithRaycast))]
	public class PlayerMover : MonoBehaviour
	{
		private CharacterController charaController;
		private GameObject fpsCamera;
		private Vector3 velocityVector = Vector3.zero;
		private Vector3 fpsCameraDirection = Vector3.zero;
		private CheckGroundedWithRaycast checkGroundedWithRaycast;

		[Range(0.1f, 10f)]
		private float walkSpeed = 5f;
		[Range(0.1f, 20f)]
		private float runSpeed = 10f;
		[Range(1f, 15f)]
		private float jumpSpeed = 4f;
		private float gravity;


		public void Start(){
			gravity = Physics.gravity.y;	//	-9.81
			fpsCamera = GameObject.FindWithTag("FPSCamera").gameObject;
			charaController = GetComponent<CharacterController>();
			checkGroundedWithRaycast = GetComponent<CheckGroundedWithRaycast>();	
		}

		public void Move()
		{	
			// Input
			float moveH = Input.GetAxis("Horizontal");
			float moveV = Input.GetAxis("Vertical");
			bool runKey = (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) ? true : false;
			bool jumpKey = Input.GetKeyDown(KeyCode.Space);

			// FPSCamera単位方向ベクトル
			fpsCameraDirection = fpsCamera.transform.forward * moveV + fpsCamera.transform.right * moveH;
			if(fpsCameraDirection != Vector3.zero)
			{
				fpsCameraDirection.Normalize();
			}

			// 速度ベクトルの方向をFPSCamera方向に設定
			velocityVector.x = fpsCameraDirection.x;
			velocityVector.z = fpsCameraDirection.z;

			// Run
			if(runKey)
			{
				velocityVector.x *= runSpeed;
				velocityVector.z *= runSpeed;
			}
			// Walk
			else
			{
				velocityVector.x *= walkSpeed;
				velocityVector.z *= walkSpeed;
			}


			// Jump処理
			// 接地時
			if (checkGroundedWithRaycast.CheckGrounded())
			{
				// Jump
				if(jumpKey)
				{
					velocityVector.y = jumpSpeed;
					// Debug.Log("Jump");
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
