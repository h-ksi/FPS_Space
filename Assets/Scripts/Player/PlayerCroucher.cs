using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class PlayerCroucher : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _fpsCameraTransform;

        private const float CROUCH_CAMERA_HEIGHT = 0f;  // しゃがんだときのFPSCamera Transform.localPositionのY座標
        private const float NORMAL_CAMERA_HEIGHT = 0.5f;  // 通常時のFPSCamera Transform.localPositionのY座標
        private const float CROUCH_CHARACON_HEIGHT = 1.3f;  // しゃがんだときのCharacterController.height
        private const float NORMAL_CHARACON_HEIGHT = 1.8f;  // 通常時のCharacterController.height

        private bool _isPlayerCrouching = false;
        public bool IsPlayerCrouching { get { return _isPlayerCrouching; } }

        void Update()
        {
            if (_playerController.IsCrouchCommandActive)
            {
                if (!_isPlayerCrouching)
                {
                    CrouchPlayer();
                }
                else if (_isPlayerCrouching)
                {
                    StandPlayer();
                }
            }
        }

        private void CrouchPlayer()
        {
            _fpsCameraTransform.localPosition = new Vector3(0, CROUCH_CAMERA_HEIGHT, 0);
            _characterController.height = CROUCH_CHARACON_HEIGHT;
            _isPlayerCrouching = true;
        }

        private void StandPlayer()
        {
            _fpsCameraTransform.localPosition = new Vector3(0, NORMAL_CAMERA_HEIGHT, 0);
            _characterController.height = NORMAL_CHARACON_HEIGHT;
            _isPlayerCrouching = false;
        }
    }
}