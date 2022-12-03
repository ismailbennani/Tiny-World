using System.Collections;
using Map.Tile.Animation;
using UnityEngine;

namespace Map.Tile
{
    public class MapTileResource: MonoBehaviour
    {
        private Vector2Int _position;

        private IMapTileResourceAnimation _animation;

        void OnEnable()
        {
            TryGetComponent(out _animation);
        }

        public void SetTile(TileState state)
        {
            _position = state.position;
            StartCoroutine(UpdateWhenGameStateReady());
        }

        public void OnLoot()
        {
            _animation?.OnLoot();
        }

        private IEnumerator UpdateWhenGameStateReady()
        {
            while (!GameStateManager.Ready)
            {
                yield return null;
            }

            MapState map = GameStateManager.Current.map;
            
            SetSize(map.runtimeConfig.resourceScale);
        }

        private void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }
    }
}
