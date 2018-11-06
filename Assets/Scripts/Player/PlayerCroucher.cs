using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	public class PlayerCroucher : MonoBehaviour {
		[SerializeField]private CharacterController characterController;
		[SerializeField]private Transform fpsCameraTransform;

		private const float CROUCH_CAMERA_HEIGHT = 0f;  // しゃがんだときのカメラ位置
		private const float NORMAL_CAMERA_HEIGHT = 0.5f;  // 通常時のカメラ位置
		private const float CROUCH_CHARACON_HEIGHT = 1.3f;  // しゃがんだときのCharacterController.height
		private const float NORMAL_CHARACON_HEIGHT = 1.8f;  // 通常時のCharacterController.height

		public void CrouchPlayer(bool crouchKey){
			if (crouchKey){
				fpsCameraTransform.localPosition = new Vector3(0, CROUCH_CAMERA_HEIGHT, 0);
				characterController.height = CROUCH_CHARACON_HEIGHT;
			}
			else
			{
				fpsCameraTransform.localPosition = new Vector3(0, NORMAL_CAMERA_HEIGHT, 0);
				characterController.height = NORMAL_CHARACON_HEIGHT;
			}
		}
	}
}