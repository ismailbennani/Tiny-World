using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Player
{
	public class PlayerControllerInputSource : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public bool jump;
		public bool sprint;
		public float look;
		public float zoom;

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

		public void OnLook(InputValue value)
		{
			LookInput(value.Get<float>());
		}

		public void OnZoom(InputValue value)
		{
			ZoomInput(value.Get<float>());
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

		private void LookInput(float delta)
		{
			look = delta;
		}

		private void ZoomInput(float delta)
		{
			zoom = delta;
		}
	}
}