using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(CheckGroundedWithRaycast))]
	public class PlayerController : MonoBehaviour
    {
        [SerializeField]private PlayerMover playerMover;
        [SerializeField]private PlayerCroucher playerCroucher;
        [SerializeField]private CheckGroundedWithRaycast checkGroundedWithRaycast;
        [SerializeField]private Transform fpsCameraTransform;
        private ArgumentsOfMovePlayer argumentsOfMovePlayer;
        
        private const float DEFAULT_WALK_SPEED = 5f;       
        private const float RUN_SPEED = 10f;
        private const float CROUCH_WALK_SPEED = 3f;
        private const float CROUCH_RUN_SPEED = 6f;

        

        void Start()
        {            
            argumentsOfMovePlayer = ArgumentsOfMovePlayer.SingletonInstance;
        }

        void Update()
        {            
            HandleInput();
            playerMover.MovePlayer(
                fpsCameraTransform, 
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
            bool crouchKey = Input.GetKey(KeyCode.C);

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
            if(!runnableKey && !crouchKey)
            {
                argumentsOfMovePlayer.Speed = DEFAULT_WALK_SPEED;
            }
            else if(runnableKey && !crouchKey)
            {
                argumentsOfMovePlayer.Speed = RUN_SPEED;
            }
            else if(!runnableKey && crouchKey)
            {
                argumentsOfMovePlayer.Speed = CROUCH_WALK_SPEED;
            }
            else
            {
                argumentsOfMovePlayer.Speed = CROUCH_RUN_SPEED;
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

            // Crouch
            playerCroucher.CrouchPlayer(crouchKey);
        }
    }
}
