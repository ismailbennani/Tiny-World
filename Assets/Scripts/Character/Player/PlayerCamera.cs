using Cinemachine;
using UnityEngine;

namespace Character.Player
{
    public class PlayerCamera: MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        
        [Header("Zoom")]
        public Vector3 minOffset = new(0, -5, -5);
        public Vector3 maxOffset = new(0, -20, -20);

        private CinemachineTransposer _transposer;

        private void OnEnable()
        {
            if (!virtualCamera)
            {
                GetComponent<CinemachineVirtualCamera>();
            }

            _transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void LateUpdate()
        {
            PlayerController controller = PlayerController.Instance;
            if (!controller)
            {
                return;
            }

            if (_transposer)
            {
                Vector3 zoom = Vector3.Lerp(minOffset, maxOffset, controller.zoomLevel);
                _transposer.m_FollowOffset = zoom;
            }
        }
    }
}
