using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS{
	public class CameraController: MonoBehaviour{
		[Range(0.1f, 10f)]
		public float lookSensitivity = 5f;
		[Range(0.1f, 1f)]
		public float lookSmooth = 0.5f;

		public Vector2 MinMaxAngle = new Vector2(-75, 75);

		private float yRot;
		private float xRot;

		private float currentYRot;
		private float currentXRot;

		private float yRotVelocity;
		private float xRotVelocity;

		void Update(){
			yRot += Input.GetAxis("Mouse X") * lookSensitivity;
			xRot -= Input.GetAxis("Mouse Y") * lookSensitivity;

			// Clampは与えられた最小float値と最大float値の範囲に値を制限する
			xRot = Mathf.Clamp(xRot, MinMaxAngle.x, MinMaxAngle.y);

			currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, lookSmooth);
			currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, lookSmooth);

			transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
		}		

	}
}
