using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(PlayerMover))]
	public class PlayerController : MonoBehaviour
    {
        private PlayerMover playerMover;
        private CheckGroundedWithRaycast checkGroundedWithRaycast;
        private Transform fpsCameraTransform;
        private ArgumentsOfMovePlayer argumentsOfMovePlayer;
        
        void Start()
        {
            playerMover = GetComponent<PlayerMover>();
            fpsCameraTransform = transform.GetChild(0).gameObject.transform;
            checkGroundedWithRaycast = GetComponent<CheckGroundedWithRaycast>();
            argumentsOfMovePlayer = ArgumentsOfMovePlayer.Singleton;
        }

        void Update()
        {            
            HandleInput();
            playerMover.MovePlayer
            (
                argumentsOfMovePlayer.FpsCameraTransform, 
                argumentsOfMovePlayer.MoveV, 
                argumentsOfMovePlayer.MoveH, 
                argumentsOfMovePlayer.Speed, 
                argumentsOfMovePlayer.IsGrounded, 
                argumentsOfMovePlayer.JumpKey
            );
        }

        public void HandleInput()
        {
            // Keys
            bool forwardKey = Input.GetKey (KeyCode.W);
            bool backwardKey = Input.GetKey(KeyCode.S);
            bool leftKey = Input.GetKey(KeyCode.A);
            bool rightKey = Input.GetKey(KeyCode.D);
            bool runnableKey = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? true : false;
            bool jumpKey = Input.GetKeyDown(KeyCode.Space);

            // fpsCameraTransform
            argumentsOfMovePlayer.FpsCameraTransform = fpsCameraTransform;

            // Vertical direction
            if (forwardKey && !backwardKey)
            {
                argumentsOfMovePlayer.MoveV = 1f;
            }
            else if(!forwardKey && backwardKey)
            {
                argumentsOfMovePlayer.MoveV = -1f;
            }
            else
            {
                argumentsOfMovePlayer.MoveV = 0;
            }

            // Horizontal direction
            if (!leftKey && rightKey)
            {
                argumentsOfMovePlayer.MoveH = 1f;
            }
            else if(leftKey && !rightKey)
            {
                argumentsOfMovePlayer.MoveH = -1f;
            }
            else
            {
                argumentsOfMovePlayer.MoveH = 0;
            }

            // Speed
            if(runnableKey)
            {
                argumentsOfMovePlayer.Speed = argumentsOfMovePlayer.RunSpeed;
            }
            else
            {
                argumentsOfMovePlayer.Speed = argumentsOfMovePlayer.WalkSpeed;
            }

            // isGrounded
            argumentsOfMovePlayer.IsGrounded = checkGroundedWithRaycast.CheckPlayerIsGrounded();

            // Jump
            if(jumpKey)
            {
                argumentsOfMovePlayer.JumpKey = true;
            }
            else
            {
                argumentsOfMovePlayer.JumpKey = false;
            }
        }
    }
}
