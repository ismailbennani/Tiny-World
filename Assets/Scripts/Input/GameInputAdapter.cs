using Character.Player;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    public class GameInputAdapter : MonoBehaviour
    {
        public static GameInputAdapter Instance { get; private set; }

        public PlayerInput playerInput;

        private PlayerControllerInputSource _playerControllerInputSource;

        private void OnEnable()
        {
            Instance = this;

            playerInput = GetComponent<PlayerInput>();
        }

        public void SwitchToUi()
        {
            if (!playerInput)
            {
                return;
            }
            
            playerInput.SwitchCurrentActionMap("UI");
        }

        public void SwitchToPlayer()
        {
            if (!playerInput)
            {
                return;
            }
            
            playerInput.SwitchCurrentActionMap("Player");
        }

        public void OnMove(InputValue value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.MoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.JumpInput(value.isPressed);
        }

        public void OnLook(InputValue value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.LookInput(value.Get<float>());
        }

        public void OnZoom(InputValue value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.ZoomInput(value.Get<float>());
        }

        public void OnMenu(InputValue value)
        {
            if (value.isPressed)
            {
                UIMainMenu.Instance.Toggle();
            }
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

        private bool GetPlayerController()
        {
            return ComponentExtensions.GetComponent(
                ref _playerControllerInputSource,
                () => PlayerController.Instance ? PlayerController.Instance.playerControllerInputSource : null
            );
        }
    }
}
