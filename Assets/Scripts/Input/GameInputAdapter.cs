using Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class GameInputAdapter: MonoBehaviour
    {
        public static GameInputAdapter Instance { get; private set; }
        
        public PlayerInput playerInput;
        
        private PlayerControllerInputSource _playerControllerInputSource;

        private void OnEnable()
        {
            Instance = this;
            
            playerInput = GetComponent<PlayerInput>();
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

        public void OnLook(InputValue value)
        {
            _playerControllerInputSource.LookInput(value.Get<float>());
        }

        public void OnZoom(InputValue value)
        {
            _playerControllerInputSource.ZoomInput(value.Get<float>());
        }

        public void OnSprint(InputValue value)
        {
            if (value.isPressed)
            {
                Trigger(GameInputType.ToggleSprint);
            }
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
