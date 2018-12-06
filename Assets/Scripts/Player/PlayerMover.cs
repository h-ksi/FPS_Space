using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CheckGroundedWithRaycast _checkGroundedWithRaycast;
        [SerializeField] private PlayerCroucher _playerCroucher;

        // Constants
        private const float DEFAULT_WALK_SPEED = 10f;
        private const float RUN_SPEED = 16f;
        private const float CROUCH_WALK_SPEED = 8f;
        private const float CROUCH_RUN_SPEED = 12f;
        private const float JUMP_SPEED = 3f;
        private const float GRAVITY = -20f;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _velocityVector = Vector3.zero;
        private float _moveV;   // 前進：1   後退：-1   他：0
        private float _moveH;   // 右移動：1    左移動：-1   他：0
        private float _speed;   // 水平面移動速度

        void Start()
        {
            // 前後移動コマンドで_moveVを更新
            Observable.CombineLatest(
                    _playerController.IsMoveForwardCommandActive,
                    _playerController.IsMoveBackwardCommandActive)
                .Subscribe(_boolList =>
                {
                    _moveV = SetValueOfMoveVH(_boolList[0], _boolList[1]);
                });

            // 左右移動コマンドで_moveHを更新
            Observable.CombineLatest(
                    _playerController.IsMoveRightCommandActive,
                    _playerController.IsMoveLeftCommandActive)
                .Subscribe(_boolList =>
                {
                    _moveH = SetValueOfMoveVH(_boolList[0], _boolList[1]);
                });

            // Runコマンド、しゃがみコマンドで_speedを更新
            Observable.CombineLatest(
                    _playerController.IsRunnableCommandActive,
                    _playerController.IsCrouchCommandActive)
                .Subscribe(_boolList =>
                {
                    _speed = SetValueOfSpeed(_boolList[0], _boolList[1]);
                });

            // Jumpコマンドで速度ベクトルのY成分を更新
            _playerController.IsJumpCommandActive
                .Skip(1)
                .Where(_ => _checkGroundedWithRaycast.CheckPlayerIsGrounded())
                .Subscribe(_ => _velocityVector.y = JUMP_SPEED);

            // 非接地時は毎フレーム速度ベクトルのY成分を減少させる
            this.UpdateAsObservable()
                .Where(_ => !_checkGroundedWithRaycast.CheckPlayerIsGrounded())
                .Subscribe(_ =>
                {
                    _velocityVector.y += GRAVITY * Time.fixedDeltaTime;
                });

            // Player移動
            this.UpdateAsObservable()
                // 接地時かつすべてのPlayer移動コマンドがfalseのとき、MovePlayer()を実行しない
                .SkipWhile(_ =>
                    (_checkGroundedWithRaycast.CheckPlayerIsGrounded()) &&
                    (_playerController.RPArrayNeededForMovePlayer.All(x => x.Value = false)))
                .Subscribe(_ => MovePlayer());
        }

        private void MovePlayer()
        {
            _moveDirection = _cameraController.FpsCameraHorizontalDirection.Value * _moveV + Vector3.Cross(_cameraController.FpsCameraHorizontalDirection.Value, Vector3.down) * _moveH;

            _velocityVector.x = _speed * _moveDirection.x;
            _velocityVector.z = _speed * _moveDirection.z;

            _characterController.Move(_velocityVector * Time.fixedDeltaTime);
        }

        private float SetValueOfMoveVH(bool _forwardOrRight, bool _backwardOrLeft)
        {
            if (_forwardOrRight && !_backwardOrLeft)
            {
                return 1f;
            }
            else if (!_forwardOrRight && _backwardOrLeft)
            {
                return -1f;
            }
            else
            {
                return 0;
            }
        }

        private float SetValueOfSpeed(bool _run, bool _crouch)
        {
            if (!_run && !_crouch)
            {
                return DEFAULT_WALK_SPEED;
            }
            else if (_run && !_crouch)
            {
                return RUN_SPEED;
            }
            else if (!_run && _crouch)
            {
                return CROUCH_WALK_SPEED;
            }
            else
            {
                return CROUCH_RUN_SPEED;
            }
        }
    }
}
