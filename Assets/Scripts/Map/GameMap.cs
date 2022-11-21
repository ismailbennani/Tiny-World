using System.Collections.Generic;
using System.Linq;
using Character.Player;
using Map.Chunk;
using UnityEngine;

namespace Map
{
    public class GameMap : MonoBehaviour
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
            Debug.Log("UPDATE");
            
            centerChunkPosition = player.playerChunk;

            Vector2Int loadedChunksDim = 2 * map.runtimeConfig.chunkRange + Vector2Int.one;

            int nChunks = loadedChunksDim.x * loadedChunksDim.y;

            HashSet<Vector2Int> needsRendering = new();
            for (int x = 0; x < loadedChunksDim.x; x++)
            for (int y = 0; y < loadedChunksDim.y; y++)
            {
                needsRendering.Add(new Vector2Int(x, y) - map.runtimeConfig.chunkRange - Vector2Int.one + centerChunkPosition);
            }

            chunks ??= new List<MapChunk>();

            HashSet<Vector2Int> toBeSkipped = new();
            foreach (MapChunk chunk in chunks)
            {
                if (needsRendering.Contains(chunk.state.position))
                {
                    toBeSkipped.Add(chunk.state.position);
                    needsRendering.Remove(chunk.state.position);
                }
            }

            for (int i = chunks.Count; i < nChunks; i++)
            {
                MapChunk newChunk = Instantiate(map.runtimeConfig.baseChunk, transform);
                chunks.Add(newChunk);

                Vector2Int positionForCurrentChunk = needsRendering.First();
                needsRendering.Remove(positionForCurrentChunk);

                newChunk.Set(map, positionForCurrentChunk);
            }

            foreach (MapChunk chunk in chunks)
            {
                chunk.gameObject.SetActive(true);

                if (toBeSkipped.Contains(chunk.state.position))
                {
                    continue;
                }

                if (needsRendering.Count == 0)
                {
                    chunk.gameObject.SetActive(false);
                    continue;
                }
                
                Vector2Int positionForCurrentChunk = needsRendering.First();
                needsRendering.Remove(positionForCurrentChunk);

                chunk.Set(map, positionForCurrentChunk);
            }
        }
    }
}
