using Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class GameInputAdapter: MonoBehaviour
    {
        private PlayerControllerInputSource _playerControllerInputSource;

        private void OnEnable()
        {
            _playerControllerInputSource = GetComponent<PlayerControllerInputSource>();
        }

        public void OnMove(InputValue value)
        {
            _playerControllerInputSource.MoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            _playerControllerInputSource.JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            _playerControllerInputSource.SprintInput(value.isPressed);
        }

        public void OnLook(InputValue value)
        {
            _playerControllerInputSource.LookInput(value.Get<float>());
        }

        public void OnZoom(InputValue value)
        {
            _playerControllerInputSource.ZoomInput(value.Get<float>());
        }
        
        public void OnInteract(InputValue value)
        {
            if (value.isPressed)
            {
                Trigger(GameInputType.Interact);
            }
        }

        private void Trigger(GameInputType inputType)
        {
            SendMessage("OnInput", inputType);
        }
    }
}
