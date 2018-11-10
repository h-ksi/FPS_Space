using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(CheckGroundedWithRaycast))]
    public class PlayerController : MonoBehaviour
    {

        // Key Inputs
        private bool _isMoveForwardCommandActive;
        public bool IsMoveForwardCommandActive { get { return _isMoveForwardCommandActive; } }
        private bool _isMoveBackwardCommandActive;
        public bool IsMoveBackwardCommandActive { get { return _isMoveBackwardCommandActive; } }
        private bool _isMoveLeftCommandActive;
        public bool IsMoveLeftCommandActive { get { return _isMoveLeftCommandActive; } }
        private bool _isMoveRightCommandActive;
        public bool IsMoveRightCommandActive { get { return _isMoveRightCommandActive; } }
        private bool _isRunnableCommandActive;
        public bool IsRunnableCommandActive { get { return _isRunnableCommandActive; } }
        private bool _isJumpCommandActive;
        public bool IsJumpCommandActive { get { return _isJumpCommandActive; } }
        private bool _isCrouchCommandActive;
        public bool IsCrouchCommandActive { get { return _isCrouchCommandActive; } }

        void Update()
        {
            _isMoveForwardCommandActive = Input.GetKey(KeyCode.W);
            _isMoveBackwardCommandActive = Input.GetKey(KeyCode.S);
            _isMoveLeftCommandActive = Input.GetKey(KeyCode.A);
            _isMoveRightCommandActive = Input.GetKey(KeyCode.D);
            _isRunnableCommandActive = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? true : false;
            _isJumpCommandActive = Input.GetKeyDown(KeyCode.Space);
            _isCrouchCommandActive = Input.GetKeyDown(KeyCode.C);
        }
    }
}
