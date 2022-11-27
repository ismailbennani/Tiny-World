using System;
using UnityEngine;

namespace Character.Player
{
    [Serializable]
    public class PlayerConfig: CharacterConfig
    {
        [Header("Camera")]
        public float cameraRotationSmoothTime = 0.1f;
        public float zoomSpeed = 0.1f;
    }
}
