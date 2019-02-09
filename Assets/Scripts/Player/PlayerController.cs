using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        // Singleton
        public static PlayerController SingletonInstance { get; private set; }

        public ReactiveProperty<bool>[] ArrayNeededForMovePlayerRP { get; private set; } = new ReactiveProperty<bool>[6];

        // Key Inputs
        public ReactiveProperty<bool> IsMoveForwardCommandActive { get; private set; } = new ReactiveProperty<bool> (false);
        public ReactiveProperty<bool> IsMoveBackwardCommandActive { get; private set; } = new ReactiveProperty<bool> (false);
        public ReactiveProperty<bool> IsMoveLeftCommandActive { get; private set; } = new ReactiveProperty<bool> (false);
        public ReactiveProperty<bool> IsMoveRightCommandActive { get; private set; } = new ReactiveProperty<bool> (false);
        public ReactiveProperty<bool> IsRunnableCommandActive { get; private set; } = new ReactiveProperty<bool> (false);
        public ReactiveProperty<bool> IsJumpCommandActive { get; private set; } = new ReactiveProperty<bool> (false);
        public ReactiveProperty<bool> IsCrouchCommandActive { get; private set; } = new ReactiveProperty<bool> (false);

        void Start ()
        {
            SingletonInstance = this;

            // ユーザー入力
            this.UpdateAsObservable ()
                .Subscribe (_ =>
                {
                    IsMoveForwardCommandActive.Value = Input.GetKey (KeyCode.W);
                    IsMoveBackwardCommandActive.Value = Input.GetKey (KeyCode.S);
                    IsMoveLeftCommandActive.Value = Input.GetKey (KeyCode.A);
                    IsMoveRightCommandActive.Value = Input.GetKey (KeyCode.D);
                    IsRunnableCommandActive.Value = (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) ? true : false;
                    IsJumpCommandActive.Value = Input.GetKeyDown (KeyCode.Space);
                    IsCrouchCommandActive.Value = Input.GetKeyDown (KeyCode.C);
                });

            // 値が変更されたらRPArrayNeededForMovePlayerの要素を更新
            IsMoveForwardCommandActive.Subscribe (_ => ArrayNeededForMovePlayerRP[0] = IsMoveForwardCommandActive);
            IsMoveBackwardCommandActive.Subscribe (_ => ArrayNeededForMovePlayerRP[1] = IsMoveBackwardCommandActive);
            IsMoveLeftCommandActive.Subscribe (_ => ArrayNeededForMovePlayerRP[2] = IsMoveLeftCommandActive);
            IsMoveRightCommandActive.Subscribe (_ => ArrayNeededForMovePlayerRP[3] = IsMoveRightCommandActive);
            IsRunnableCommandActive.Subscribe (_ => ArrayNeededForMovePlayerRP[4] = IsRunnableCommandActive);
            IsJumpCommandActive.Subscribe (_ => ArrayNeededForMovePlayerRP[5] = IsJumpCommandActive);
        }
    }
}