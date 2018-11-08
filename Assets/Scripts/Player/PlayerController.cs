using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(CheckGroundedWithRaycast))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMover playerMover;
        [SerializeField] private PlayerCroucher playerCroucher;
        [SerializeField] private CheckGroundedWithRaycast checkGroundedWithRaycast;
        [SerializeField] private Transform fpsCameraTransform;
        private ArgumentsOfMovePlayer argumentsOfMovePlayer;

        // Constants
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
                argumentsOfMovePlayer.isJumpCommandActive
            );
        }

        public void HandleInput()
        {            
            // Keys
            bool isMoveForwardCommandActive = Input.GetKey(KeyCode.W);
            bool isMoveBackwardCommandActive = Input.GetKey(KeyCode.S);
            bool isMoveLeftCommandActive = Input.GetKey(KeyCode.A);
            bool isMoveRightCommandActive = Input.GetKey(KeyCode.D);
            bool isRunnableCommandActive = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? true : false;
            bool isJumpCommandActive = Input.GetKeyDown(KeyCode.Space);
            bool isCrouchCommandActive = Input.GetKey(KeyCode.C);

            // Vertical direction
            if (isMoveForwardCommandActive && !isMoveBackwardCommandActive)
            {
                argumentsOfMovePlayer.MoveV = 1f;
            }
            else if (!isMoveForwardCommandActive && isMoveBackwardCommandActive)
            {
                argumentsOfMovePlayer.MoveV = -1f;
            }
            else
            {
                argumentsOfMovePlayer.MoveV = 0;
            }

            // Horizontal direction
            if (!isMoveLeftCommandActive && isMoveRightCommandActive)
            {
                argumentsOfMovePlayer.MoveH = 1f;
            }
            else if (isMoveLeftCommandActive && !isMoveRightCommandActive)
            {
                argumentsOfMovePlayer.MoveH = -1f;
            }
            else
            {
                argumentsOfMovePlayer.MoveH = 0;
            }

            // Speed
            if (!isRunnableCommandActive && !isCrouchCommandActive)
            {
                argumentsOfMovePlayer.Speed = DEFAULT_WALK_SPEED;
            }
            else if (isRunnableCommandActive && !isCrouchCommandActive)
            {
                argumentsOfMovePlayer.Speed = RUN_SPEED;
            }
            else if (!isRunnableCommandActive && isCrouchCommandActive)
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
            if (isJumpCommandActive)
            {
                argumentsOfMovePlayer.isJumpCommandActive = true;
            }
            else
            {
                argumentsOfMovePlayer.isJumpCommandActive = false;
            }

            // Crouch
            playerCroucher.HandlePlayerCrouch(isCrouchCommandActive);
        }
    }
}
