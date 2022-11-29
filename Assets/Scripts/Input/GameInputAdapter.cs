using Character.Player;
using UI;
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

        public void OnMove(InputAction.CallbackContext value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.MoveInput(value.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.JumpInput(value.ReadValueAsButton());
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.LookInput(value.ReadValue<float>());
        }

        public void OnZoom(InputAction.CallbackContext value)
        {
            if (!GetPlayerController())
            {
                return;
            }

            _playerControllerInputSource.ZoomInput(value.ReadValue<float>());
        }

        public void OnMenu(InputAction.CallbackContext value)
        {
            if (value.ReadValueAsButton())
            {
                UIManager.Instance.Toggle();
            }
        }

        public void OnSprint(InputAction.CallbackContext value)
        {
            if (value.ReadValueAsButton())
            {
                Trigger(GameInputType.ToggleSprint);
            }
        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (value.ReadValueAsButton())
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
