using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class GameInputAdapter: MonoBehaviour
    {
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
