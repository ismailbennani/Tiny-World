using UnityEngine;

namespace Character.Player
{
    public class FollowPlayer : MonoBehaviour
    {
        public Vector2 minOffset = new(-5, -5);
        public Vector2 maxOffset = new(-20, -20);

        void LateUpdate()
        {
            PlayerController playerController = PlayerController.Instance;
            if (!playerController)
            {
                return;
            }

            Vector3 offset = Vector3.Slerp(minOffset, maxOffset, playerController.zoomLevel);
            Vector3 playerPosition = playerController.transform.position;
            
            transform.position = new Vector3(playerPosition.x, 0, playerPosition.z) - offset;
            transform.rotation = Quaternion.Euler(0, playerController.cameraAngle, 0);
        }
    }
}
