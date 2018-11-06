using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    // Transformを除く「MovePlayerメソッドの引数」を保存するクラス
    public class ArgumentsOfMovePlayer
    {
        private static ArgumentsOfMovePlayer singletonInstance = new ArgumentsOfMovePlayer();
        public static ArgumentsOfMovePlayer SingletonInstance{get{return singletonInstance;}}
        
        public float MoveV{get; set;}
        public float MoveH{get; set;}
        public float Speed{get; set;}
        public bool IsGrounded{get; set;}
        public bool JumpKey{get; set;}
    }
}