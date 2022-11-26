using UnityEngine;

namespace Utils
{
    public static class CameraExtensions
    {
        public static bool InViewport(this Camera camera, Vector3 position)
        {
            Vector3 viewport = camera.WorldToViewportPoint(position);

            return viewport.z > 0 && viewport.x is >= 0 and <= 1 && viewport.y is >= 0 and <= 1;
        }
    }
}
