using System.Linq;
using UnityEngine;

namespace Map.Tile.Animation
{
    public class MapTileResourceAnimateChildren : MonoBehaviour, IMapTileResourceAnimation
    {
        public GameObject[] children = { };

        private IMapTileResourceAnimation[] _childrenAnimations = { };

        private void OnEnable()
        {
            _childrenAnimations = children.Select(c => c.TryGetComponent(out IMapTileResourceAnimation anim) ? anim : null)
                .Where(a => a != null).ToArray();
        }

        public void OnLoot()
        {
            foreach (IMapTileResourceAnimation child in _childrenAnimations)
            {
                child.OnLoot();
            }
        }
    }
}
