using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
	public class PlayerMover : MonoBehaviour
	{
		public enum PlayerState
		{
			Idle, 
			Walking, 
			Running, 
			Jumping
		}

		private CharacterController charaController;
		private GameObject FPSCamera;
		private Vector3 moveDir = Vector3.zero;

		[Range(0.1f, 2f)]
		[SerializeField] private float walkSpeed = 1.5f;
		[Range(0.1f, 10f)]
		[SerializeField] private float runSpeed = 2.5f;

		[Range(0.1f, 10f)]
		[SerializeField] private float gravity = 9.81f;
		[Range(1f, 15f)]
		[SerializeField] private float jumpPower = 5f;

		public void Start(){
			FPSCamera = GameObject.Find("FPSCamera");
			charaController = GetComponent<CharacterController>();
		}

		public void Move(float moveH, float moveV)
		{
			Vector3 movement = new Vector3(moveH, 0, moveV);

			if(movement.sqrMagnitude > 1)
			{
				movement.Normalize();
			}

			Vector3 direction = FPSCamera.transform.forward * movement.z + FPSCamera.transform.right * movement.x;
			moveDir.x = direction.x * 5f;
			moveDir.z = direction.z * 5f;
			// Run
			if(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)){
				charaController.Move(moveDir * Time.fixedDeltaTime * runSpeed);
			}
			else{
				charaController.Move(moveDir * Time.fixedDeltaTime * walkSpeed);
			}

			// 落下加速度の調整
			moveDir.y -= gravity * Time.deltaTime * 2f;
			if(charaController.isGrounded){
				if(Input.GetKeyDown(KeyCode.Space)){
					moveDir.y = jumpPower;
				}
			}
		}
	}
}
