using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour {
	private Transform playerTransform;
	private Transform cameraTransform;
	private float EulerAngleLimit;

	private float speed = 7f;
	
	// Use this for initialization
	void Start () {

		// Playerタグのつけ忘れに注意！
    	playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // 見つからない場合は自身を設定
        if(playerTransform == null){
            playerTransform = gameObject.transform;
			Debug.Log("Can't find the tag 'Player'");
		}
		cameraTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		float X_Rotation = Input.GetAxis("Mouse X");
		float Y_Rotation = Input.GetAxis("Mouse Y");
		
		//	y軸回転
		playerTransform.Rotate(0, X_Rotation * speed, 0);
		
		//	x軸回転
		cameraTransform.Rotate(-Y_Rotation * speed, 0, 0);


		

		
	}
	
}
