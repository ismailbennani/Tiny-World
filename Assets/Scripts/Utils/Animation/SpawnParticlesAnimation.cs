using System.Collections;
using UnityEngine;

namespace Utils.Animation
{
    public class SpawnParticlesAnimation: MonoBehaviour, IAnimation
    {
        public GameObject particles;
        public Transform parent;

        public TransformParameters[] positions;

        public IEnumerator Animate()
        {
            foreach (TransformParameters position in positions)
            {
                Vector3 worldPosition = transform.localToWorldMatrix.MultiplyPoint(position.position);
                GameObject newParticles = Instantiate(particles, worldPosition, Quaternion.Euler(position.rotation), parent);
                newParticles.transform.localScale = position.scale;
            }

            yield break;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (TransformParameters position in positions)
            {
                Vector3 worldPosition = transform.localToWorldMatrix.MultiplyPoint(position.position);

                Gizmos.DrawWireSphere(worldPosition, 0.05f * position.scale.magnitude);
                Gizmos.DrawLine(worldPosition, worldPosition + Quaternion.Euler(position.rotation) * Vector3.forward / 10);
            }
        }
    }
}
