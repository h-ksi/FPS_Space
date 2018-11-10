using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CheckGroundedWithRaycast _checkGroundedWithRaycast;
        [SerializeField] private Transform _fpsCameraTransform;
        [SerializeField] private PlayerCroucher _playerCroucher;
        private Vector3 _velocityVector = Vector3.zero;

        // Constants
        private const float DEFAULT_WALK_SPEED = 5f;
        private const float RUN_SPEED = 8f;
        private const float CROUCH_WALK_SPEED = 3f;
        private const float CROUCH_RUN_SPEED = 5f;
        private const float JUMP_SPEED = 3f;
        private const float GRAVITY = -9.81f;

        private float _moveV;
        private float _moveH;
        private float _speed;

        // Vertical direction
        void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            InputFromPlayerController();

            // FPSCamera単位方向ベクトル
            Vector3 _fpsCameraDirection = _fpsCameraTransform.forward * _moveV + _fpsCameraTransform.right * _moveH;
            if (_fpsCameraDirection != Vector3.zero)
            {
                _fpsCameraDirection.Normalize();
            }

            // 速度ベクトルの方向をFPSCamera方向に設定
            _velocityVector.x = _fpsCameraDirection.x * _speed;
            _velocityVector.z = _fpsCameraDirection.z * _speed;

            // 接地時
            if (_checkGroundedWithRaycast.CheckPlayerIsGrounded())
            {
                // Jump
                if (_playerController.IsJumpCommandActive)
                {
                    _velocityVector.y = JUMP_SPEED;
                }
            }
            // 非接地時
            else
            {
                // 重力による自由落下処理
                _velocityVector.y += GRAVITY * Time.fixedDeltaTime;
            }

            _characterController.Move(_velocityVector * Time.fixedDeltaTime);
        }

        private void InputFromPlayerController()
        {
            bool _isMoveForwardCommandActive = _playerController.IsMoveForwardCommandActive;
            bool _isMoveBackwardCommandActive = _playerController.IsMoveBackwardCommandActive;
            bool _isMoveLeftCommandActive = _playerController.IsMoveLeftCommandActive;
            bool _isMoveRightCommandActive = _playerController.IsMoveRightCommandActive;
            bool _isRunnableCommandActive = _playerController.IsRunnableCommandActive;
            bool _isCrouchCommandActive = _playerController.IsCrouchCommandActive;

            if (_isMoveForwardCommandActive && !_isMoveBackwardCommandActive)
            {
                _moveV = 1f;
            }
            else if (!_isMoveForwardCommandActive && _isMoveBackwardCommandActive)
            {
                _moveV = -1f;
            }
            else
            {
                _moveV = 0;
            }

            // Horizontal direction
            if (!_isMoveLeftCommandActive && _isMoveRightCommandActive)
            {
                _moveH = 1f;
            }
            else if (_isMoveLeftCommandActive && !_isMoveRightCommandActive)
            {
                _moveH = -1f;
            }
            else
            {
                _moveH = 0;
            }

            // Speed
            if (!_isRunnableCommandActive && !_isCrouchCommandActive)
            {
                _speed = DEFAULT_WALK_SPEED;
            }
            else if (_isRunnableCommandActive && !_isCrouchCommandActive)
            {
                _speed = RUN_SPEED;
            }
            else if (!_isRunnableCommandActive && _isCrouchCommandActive)
            {
                _speed = CROUCH_WALK_SPEED;
            }
            else
            {
                _speed = CROUCH_RUN_SPEED;
            }
        }
    }
}