using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class ArgumentsOfMovePlayer
    {
        private static ArgumentsOfMovePlayer singleton = new ArgumentsOfMovePlayer();
        public static ArgumentsOfMovePlayer Singleton
        {
            get{
                return singleton;
            }
        }

        private Transform fpsCameraTransform;
        public Transform FpsCameraTransform
        {
            set{this.fpsCameraTransform = value;}
            get{return this.fpsCameraTransform;}
        }

        private float moveV;
        public float MoveV
        {
            set{this.moveV = value;}
            get{return this.moveV;}
        }
        
        private float moveH;
        public float MoveH
        {
            set{this.moveH = value;}
            get{return this.moveH;}
        }
        
        private float walkSpeed = 5f;
        public float WalkSpeed{get{return this.walkSpeed;}}
        private float runSpeed = 10f;
        public float RunSpeed{get{return this.runSpeed;}}
        private float speed;
        public float Speed
        {
            set{this.speed = value;}
            get{return this.speed;}
        }

        private bool isGrounded;
        public bool IsGrounded
        {
            set{this.isGrounded = value;}
            get{return this.isGrounded;}
        }

        private bool jumpKey;
        public bool JumpKey
        {
            set{this.jumpKey = value;}
            get{return this.jumpKey;}
        }
    }
}