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
		public float walkSpeed = 1.5f;
		[Range(0.1f, 10f)]
		public float runSpeed = 3.5f;

		// Character Controller
		private CharacterController charaController;

		void Start(){
			charaController = GetComponent<CharacterController>();
		}

		void Update(){
			Move();
		}

		void Move(){
			float moveH = Input.GetAxis("Horizontal");
			float moveV = Input.GetAxis("Vertical");
			Vector3 movement = new Vector3(moveH, 0, moveV);

			if(movement.sqrMagnitude > 1){
				movement.Normalize();
			}
			
			// Run
			if(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)){
				charaController.Move(movement * Time.fixedDeltaTime * runSpeed);
			}
			else{
				charaController.Move(movement * Time.fixedDeltaTime * walkSpeed);
			}
		}
	}
}