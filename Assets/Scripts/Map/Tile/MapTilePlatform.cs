using System.Collections;
using UnityEngine;

namespace Map.Tile
{
    public class MapTilePlatform: MonoBehaviour
    {
        private TileState _state;
        
        void OnEnable()
        {
            if (_state != null)
            {
                StartCoroutine(UpdateWhenGameStateReady());
            }
        }

        public void SetTile(TileState state)
        {
            _state = state;
            StartCoroutine(UpdateWhenGameStateReady());
        }

        private IEnumerator UpdateWhenGameStateReady()
        {
            while (!GameStateManager.Current)
            {
                yield return null;
            }
            
            SetSize(GameStateManager.Current.map.config.tileSize);
        }

        private void SetSize(Vector2 size)
        {
            transform.localScale = new Vector3(size.x, 1, size.y);
        }
    }
}
