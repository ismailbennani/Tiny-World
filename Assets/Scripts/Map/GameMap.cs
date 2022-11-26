using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character.Player;
using Map.Chunk;
using Map.Tile;
using UnityEngine;

namespace Map
{
    public class GameMap : MonoBehaviour
    {
        public static GameMap Instance { get; private set; }
        
        public bool Ready { get; private set; }
        
        [SerializeField]
        private List<MapChunk> chunks;

        [SerializeField]
        private Vector2Int centerChunkPosition;
        
        private Coroutine _updateCoroutine;

        private void OnEnable()
        {
            Instance = this;
        }

        public void Initialize()
        {
            Ready = false;
            
            _updateCoroutine = StartCoroutine(UpdateChunks(GameStateManager.Current.map, GameStateManager.Current.player));
        }

        public void Update()
        {
            if (_updateCoroutine == null && GameStateManager.Current.player.playerChunk != centerChunkPosition)
            {
                _updateCoroutine = StartCoroutine(UpdateChunks(GameStateManager.Current.map, GameStateManager.Current.player));
            }
        }

        public MapTile GetTile(TileState tile)
        {
            GameState state = GameStateManager.Current;
            if (!state)
            {
                return null;
            }

            if (state.map == null)
            {
                return null;
            }

            Vector2Int chunkPosition = state.map.GetChunkPositionAt(tile.position);
            MapChunk chunk = chunks.SingleOrDefault(c => c.state.position == chunkPosition);
            return chunk.tiles.SingleOrDefault(t => t.state.position == tile.position);
        }

        private IEnumerator UpdateChunks(MapState map, PlayerState player)
        {
            centerChunkPosition = player.playerChunk;

            Vector2Int loadedChunksDim = 2 * map.runtimeConfig.chunkRange + Vector2Int.one;

            int nChunks = loadedChunksDim.x * loadedChunksDim.y;

            HashSet<Vector2Int> needsRendering = new();
            for (int x = 0; x < loadedChunksDim.x; x++)
            for (int y = 0; y < loadedChunksDim.y; y++)
            {
                needsRendering.Add(new Vector2Int(x, y) - map.runtimeConfig.chunkRange + centerChunkPosition);
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
                toBeSkipped.Add(positionForCurrentChunk);

                newChunk.Set(map, positionForCurrentChunk);
                
                yield return null;
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

                yield return null;
            }

            _updateCoroutine = null;
            Ready = true;
        }
    }
}
