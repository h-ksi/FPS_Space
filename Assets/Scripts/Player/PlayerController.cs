using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS{
	public enum PlayerState{
		Idle, Walking, Running, Jumping
	}

	[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
	public class PlayerController: MonoBehaviour{
		[Range(0.1f, 2f)]
		public float walkSpeed = 1.3f;
		[Range(0.1f, 10f)]
		public float runSpeed = 2.3f;
		[Range(0.05f, 1.2f)]
		public float crouchSpeed = 0.7f;
		[Range(0.1f, 2f)]
		public float crouchRunSpeed = 1.4f;


		// Character Controller
		private CharacterController charaController;

		private GameObject FPSCamera;
		private Vector3 moveDir = Vector3.zero;

		[Range(0.1f, 10f)]
		public float gravity = 9.81f;
		[Range(1f, 15f)]
		public float jumpPower = 5f;

		[Range(0.1f, 2f)]
		public float crouchHeight  = 1f;
		[Range(0.1f, 5f)]
		public float normalHeight = 2f;

		void Start(){
			FPSCamera = GameObject.Find("FPSCamera");
			charaController = GetComponent<CharacterController>();
		}

		void Update(){
			Move();

			Crouch();
		}

		void FixedUpdate(){

		}

		void Move(){
			float moveH = Input.GetAxis("Horizontal");
			float moveV = Input.GetAxis("Vertical");
			Vector3 movement = new Vector3(moveH, 0, moveV);

			if(movement.sqrMagnitude > 1){
				movement.Normalize();
			}

			Vector3 desiredMove = FPSCamera.transform.forward * movement.z + FPSCamera.transform.right * movement.x;
			moveDir.x = desiredMove.x * 5f;
			moveDir.z = desiredMove.z * 5f;
			// Run
			if(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)){
				charaController.Move(moveDir * Time.fixedDeltaTime * runSpeed);
			}
			// Crouch
			else if(Input.GetKey(KeyCode.C)){
				charaController.Move(moveDir * Time.fixedDeltaTime * crouchSpeed);
			}
			// Crouch Run
			else if(Input.GetKey(KeyCode.C) && (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))){
				charaController.Move(moveDir * Time.fixedDeltaTime * crouchRunSpeed);
			}
			// Walk
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

		void Crouch(){
			if(Input.GetKey(KeyCode.C)){
				charaController.height = Mathf.Lerp(normalHeight, crouchHeight, 1f);
			}
			else{
				charaController.height = normalHeight;
			}
		}
	}
}