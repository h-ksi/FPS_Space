using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
	[RequireComponent(typeof(CharacterController), typeof(PlayerMover))]
	public class PlayerController : MonoBehaviour
	{
		public enum PlayerState
		{
			Idle, 
			Walking, 
			Running, 
			Jumping
		}

		private PlayerMover playerMover;

		void Start()
		{
			playerMover = GetComponent<PlayerMover>();
		}

		void FixedUpdate()
		{
			playerMover.Move();
		}
	}
}