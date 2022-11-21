using System.Collections.Generic;
using Character.Player;
using Map.Chunk;
using UnityEngine;
using Utils;

namespace Map
{
    public class MapBuilder : MonoBehaviour
    {
        [SerializeField]
        private List<MapChunk> chunks;

        [SerializeField]
        private Vector2Int centerChunkPosition;

        public void Initialize()
        {
            UpdateChunks(GameStateManager.Current.map, GameStateManager.Current.player);
        }

        public void Update()
        {
            if (GameStateManager.Current.player.playerChunk != centerChunkPosition)
            {
                UpdateChunks(GameStateManager.Current.map, GameStateManager.Current.player);
            }
        }

        private void UpdateChunks(MapState map, PlayerState player)
        {
            centerChunkPosition = player.playerChunk;

            Vector2Int loadedChunksDim = 2 * map.runtimeConfig.chunkRange + Vector2Int.one;

            int nChunks = loadedChunksDim.x * loadedChunksDim.y;

            if (chunks == null)
            {
                chunks = new List<MapChunk>();
            }
            
            for (int i = chunks.Count; i < nChunks; i++)
            {
                MapChunk newChunk = Instantiate(map.runtimeConfig.baseChunk, transform);
                chunks.Add(newChunk);
            }

            for (int index = 0; index < nChunks; index++)
            {
                MapChunk chunk = chunks[index];
                chunk.gameObject.SetActive(true);

                (int x, int y) = MyMath.GetCoords(index, loadedChunksDim);

                Vector2Int chunkPosition = new Vector2Int(x, y) - map.runtimeConfig.chunkRange + centerChunkPosition;

                chunk.Set(map, chunkPosition);
            }

            for (int index = nChunks; index < chunks.Count; index++)
            {
                chunks[index].gameObject.SetActive(false);
            }
        }
    }
}
