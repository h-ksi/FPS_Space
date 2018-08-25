using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	private Transform player;
	public float walkSpeed = 3.0F;
	public float runSpeed = 6.0f;
    public float jumpSpeed = 8.0F;
	public float rotateSpeed = 10.0f;
    public float gravity = 25.0F;
    private Vector3 moveDirection = Vector3.zero;

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player").transform;
		if(player == null){
			player = transform;
		}
	}

    void Update() {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
				moveDirection *= runSpeed;
				Debug.Log("Shiftキーを押した");
			}
			else{
				moveDirection *= walkSpeed;
			}
			
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
            
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}
