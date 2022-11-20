using System.Collections;
using State;
using UnityEngine;

namespace Map.Tile
{
    public class MapTilePlatform: MonoBehaviour
    {
        void OnEnable()
        {
            StartCoroutine(SetSizeWhenReady());
        }

        private IEnumerator SetSizeWhenReady()
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
