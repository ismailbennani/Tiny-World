using System.Collections;
using UnityEngine;

namespace Map.Tile
{
    public class MapTilePlatform : MonoBehaviour
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

            transform.localScale = new Vector3(GameStateManager.Current.map.runtimeConfig.tileSize.x, 1, GameStateManager.Current.map.runtimeConfig.tileSize.y);
        }
    }
}
