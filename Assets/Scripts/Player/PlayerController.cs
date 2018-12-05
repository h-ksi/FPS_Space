using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        // Singleton
        public static PlayerController SingletonInstance { get; private set; }

        public ReactiveProperty<bool>[] RPArrayNeededForMovePlayer { get; private set; }


        // Key Inputs
        public ReactiveProperty<bool> IsMoveForwardCommandActive { get; private set; }
        public ReactiveProperty<bool> IsMoveBackwardCommandActive { get; private set; }
        public ReactiveProperty<bool> IsMoveLeftCommandActive { get; private set; }
        public ReactiveProperty<bool> IsMoveRightCommandActive { get; private set; }
        public ReactiveProperty<bool> IsRunnableCommandActive { get; private set; }
        public ReactiveProperty<bool> IsJumpCommandActive { get; private set; }
        public ReactiveProperty<bool> IsCrouchCommandActive { get; private set; }



        void Start()
        {
            SingletonInstance = this;

            // 初期化
            RPArrayNeededForMovePlayer = new ReactiveProperty<bool>[6];

            IsMoveForwardCommandActive = new ReactiveProperty<bool>(false);
            IsMoveBackwardCommandActive = new ReactiveProperty<bool>(false);
            IsMoveLeftCommandActive = new ReactiveProperty<bool>(false);
            IsMoveRightCommandActive = new ReactiveProperty<bool>(false);
            IsRunnableCommandActive = new ReactiveProperty<bool>(false);
            IsJumpCommandActive = new ReactiveProperty<bool>(false);

            IsCrouchCommandActive = new ReactiveProperty<bool>(false);

            // ユーザー入力
            this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        IsMoveForwardCommandActive.Value = Input.GetKey(KeyCode.W);
                        IsMoveBackwardCommandActive.Value = Input.GetKey(KeyCode.S);
                        IsMoveLeftCommandActive.Value = Input.GetKey(KeyCode.A);
                        IsMoveRightCommandActive.Value = Input.GetKey(KeyCode.D);
                        IsRunnableCommandActive.Value = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? true : false;
                        IsJumpCommandActive.Value = Input.GetKeyDown(KeyCode.Space);
                        IsCrouchCommandActive.Value = Input.GetKeyDown(KeyCode.C);
                    });

            // 値が変更されたらRPArrayNeededForMovePlayerの要素を更新
            IsMoveForwardCommandActive.Subscribe(_ => RPArrayNeededForMovePlayer[0] = IsMoveForwardCommandActive);
            IsMoveBackwardCommandActive.Subscribe(_ => RPArrayNeededForMovePlayer[1] = IsMoveBackwardCommandActive);
            IsMoveLeftCommandActive.Subscribe(_ => RPArrayNeededForMovePlayer[2] = IsMoveLeftCommandActive);
            IsMoveRightCommandActive.Subscribe(_ => RPArrayNeededForMovePlayer[3] = IsMoveRightCommandActive);
            IsRunnableCommandActive.Subscribe(_ => RPArrayNeededForMovePlayer[4] = IsRunnableCommandActive);
            IsJumpCommandActive.Subscribe(_ => RPArrayNeededForMovePlayer[5] = IsJumpCommandActive);
        }
    }
}
