using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace FPS
{
    public class CameraController : MonoBehaviour
    {
        public ReactiveProperty<Vector3> FpsCameraHorizontalDirection { get; private set; }

        [Range(0.1f, 10f)]
        private const float LOOK_SENSITIVITY = 7f;
        [Range(0.1f, 1f)]
        private const float TRANSITION_TIME = 0.08f;

        private Vector2 _minMaxAngle = new Vector2(-90, 90);

        private ReactiveProperty<float> _yRot;
        private ReactiveProperty<float> _xRot;

        private float _currentYRot;
        private float _currentXRot;
        private float _yRotVelocity;
        private float _xRotVelocity;

        void Start()
        {
            _yRot = new ReactiveProperty<float>(0);
            _xRot = new ReactiveProperty<float>(0);

            FpsCameraHorizontalDirection = new ReactiveProperty<Vector3>(transform.forward.GetHorizontalDirection());

            // Mouse Input
            this.UpdateAsObservable().Subscribe(_ =>
            {
                _yRot.Value += Input.GetAxis("Mouse X") * LOOK_SENSITIVITY;
                _xRot.Value -= Input.GetAxis("Mouse Y") * LOOK_SENSITIVITY;
            });

            Observable.CombineLatest(_yRot, _xRot)
                .Subscribe(_ =>
                {
                    // Clampで最小float値と最大float値の範囲に上下回転範囲を制限する
                    _xRot.Value = Mathf.Clamp(_xRot.Value, _minMaxAngle.x, _minMaxAngle.y);

                    _currentXRot = Mathf.SmoothDampAngle(_currentXRot, _xRot.Value, ref _xRotVelocity, TRANSITION_TIME);
                    _currentYRot = Mathf.SmoothDampAngle(_currentYRot, _yRot.Value, ref _yRotVelocity, TRANSITION_TIME);

                    transform.rotation = Quaternion.Euler(_currentXRot, _currentYRot, 0);

                    FpsCameraHorizontalDirection.Value = transform.forward.GetHorizontalDirection();
                });
        }
    }
}