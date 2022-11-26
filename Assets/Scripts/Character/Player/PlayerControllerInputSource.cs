using UnityEngine;

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

		public void LookInput(float delta)
		{
			look = delta;
		}

		public void ZoomInput(float delta)
		{
			zoom = delta;
		}
	}
}