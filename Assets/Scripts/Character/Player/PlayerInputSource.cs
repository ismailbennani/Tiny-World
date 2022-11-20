using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Player
{
	public class PlayerInputSource : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
	}
}