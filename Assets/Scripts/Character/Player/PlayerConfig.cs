using UnityEngine;

namespace Character.Player
{
    [CreateAssetMenu(menuName = "Custom/Player config")]
    public class PlayerConfig: CharacterConfig
    {
        [Header("Camera")]
        public float cameraRotationSmoothTime = 0.1f;
        public float zoomSpeed = 0.1f;
    }
}
