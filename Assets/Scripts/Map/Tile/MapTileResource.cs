using System.Collections;
using UnityEngine;

namespace Map.Tile
{
    public class MapTileResource: MonoBehaviour
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
            while (!GameStateManager.Ready)
            {
                yield return null;
            }

            MapState map = GameStateManager.Current.map;
            
            SetSize(map.config.resourceScale);
            transform.rotation = Quaternion.Euler(0, 90 * _state.rotation, 0);
        }

        private void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }
    }
}
