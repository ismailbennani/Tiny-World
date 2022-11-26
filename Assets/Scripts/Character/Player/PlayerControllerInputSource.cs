using System.Collections;
using Input;
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

        private Coroutine _coroutine;
        
        void OnEnable()
        {
            _coroutine = StartCoroutine(RegisterSprintInputWhenReady());
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void LookInput(float delta)
        {
            look = delta;
        }

        public void ZoomInput(float delta)
        {
            zoom = delta;
        }

        public void ToggleSprint()
        {
            sprint = !sprint;

            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(RegisterSprintInputWhenReady());
            }
        }

        private IEnumerator RegisterSprintInputWhenReady()
        {
            while (!GameInputCallbackManager.Instance)
            {
                yield return null;
            }

            GameInputCallback callback = new(sprint ? "Walk" : "Sprint", ToggleSprint);

            GameInputCallbackManager.Instance.Register(GameInputType.ToggleSprint, this, callback);

            _coroutine = null;
        }
    }
}
