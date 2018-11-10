using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class CameraController : MonoBehaviour
    {
        [Range(0.1f, 10f)]
        private float _lookSensitivity = 5f;
        [Range(0.1f, 1f)]
        private float _lookSmooth = 0.1f;

        private Vector2 _minMaxAngle = new Vector2(-75, 75);

        private float _yRot;
        private float _xRot;

        private float _currentYRot;
        private float _currentXRot;

        private float _yRotVelocity;
        private float _xRotVelocity;

        void Update()
        {
            _yRot += Input.GetAxis("Mouse X") * _lookSensitivity;
            _xRot -= Input.GetAxis("Mouse Y") * _lookSensitivity;

            // Clampは与えられた最小float値と最大float値の範囲に値を制限する
            _xRot = Mathf.Clamp(_xRot, _minMaxAngle.x, _minMaxAngle.y);

            _currentXRot = Mathf.SmoothDamp(_currentXRot, _xRot, ref _xRotVelocity, _lookSmooth);
            _currentYRot = Mathf.SmoothDamp(_currentYRot, _yRot, ref _yRotVelocity, _lookSmooth);

            transform.rotation = Quaternion.Euler(_currentXRot, _currentYRot, 0);
        }

    }
}