using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
	public class PlayerController : MonoBehaviour
	{
		private float moveH;
		private float moveV;

		void Start()
		{

		}

		void Update()
		{
			moveH = Input.GetAxis("Horizontal");
			moveV = Input.GetAxis("Vertical");
			if(moveH != 0 || moveV != 0)
			{
				GetComponent<PlayerMover>().Move(moveH, moveV);
			}
		}
	}
}