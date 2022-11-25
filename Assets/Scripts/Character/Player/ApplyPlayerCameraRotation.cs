using UnityEngine;

namespace Character.Player
{
    public class ApplyPlayerCameraRotation : MonoBehaviour
    {
        void LateUpdate()
        {
            PlayerController playerController = PlayerController.Instance;
            if (!playerController)
            {
                return;
            }

            transform.rotation = Quaternion.Euler(0, playerController.cameraAngle, 0);
        }
    }
}
