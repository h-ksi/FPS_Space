using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class PlayerCroucher : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform fpsCameraTransform;

        private const float CROUCH_CAMERA_HEIGHT = 0f;  // しゃがんだときのFPSCamera Transform.localPositionのY座標
        private const float NORMAL_CAMERA_HEIGHT = 0.5f;  // 通常時のFPSCamera Transform.localPositionのY座標
        private const float CROUCH_CHARACON_HEIGHT = 1.3f;  // しゃがんだときのCharacterController.height
        private const float NORMAL_CHARACON_HEIGHT = 1.8f;  // 通常時のCharacterController.height

        private bool isPlayerCrouching = false;

        public void HandlePlayerCrouch(bool isCrouchCommandActive)
        {
            if (isCrouchCommandActive && !isPlayerCrouching)
            {
                CrouchPlayer();
            }
            // Don't crouch
            else if (!isCrouchCommandActive && isPlayerCrouching)
            {
                StandPlayer();
            }
        }
        private void CrouchPlayer()
        {
            fpsCameraTransform.localPosition = new Vector3(0, CROUCH_CAMERA_HEIGHT, 0);
            characterController.height = CROUCH_CHARACON_HEIGHT;
            isPlayerCrouching = true;
        }

        private void StandPlayer()
        {
            fpsCameraTransform.localPosition = new Vector3(0, NORMAL_CAMERA_HEIGHT, 0);
            characterController.height = NORMAL_CHARACON_HEIGHT;
            isPlayerCrouching = false;
        }
    }
}