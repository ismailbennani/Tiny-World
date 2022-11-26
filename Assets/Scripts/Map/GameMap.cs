using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character.Player;
using Map.Chunk;
using Map.Tile;
using UnityEngine;
using Utils;

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
        private Camera _camera;

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
            if (_updateCoroutine == null)
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

            HashSet<Vector2Int> chunksInView = new()
            {
                centerChunkPosition
            };

            if (!_camera)
            {
                _camera = Camera.main;
            }

            for (int x = -map.runtimeConfig.maxChunkRange.x; x <= map.runtimeConfig.maxChunkRange.x; x++)
            for (int y = -map.runtimeConfig.maxChunkRange.y; y <= map.runtimeConfig.maxChunkRange.y; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y) + centerChunkPosition;

                Vector3 corner1 = map.GetTileCenterPosition(chunkPosition * map.initialConfig.chunkSize);
                Vector3 corner2 = map.GetTileCenterPosition(chunkPosition * map.initialConfig.chunkSize + new Vector2Int(map.initialConfig.chunkSize.x, 0));
                Vector3 corner3 = map.GetTileCenterPosition(chunkPosition * map.initialConfig.chunkSize + map.initialConfig.chunkSize);
                Vector3 corner4 = map.GetTileCenterPosition(chunkPosition * map.initialConfig.chunkSize + new Vector2Int(0, map.initialConfig.chunkSize.y));

                bool corner1InViewport = _camera.InViewport(corner1);
                bool corner2InViewport = _camera.InViewport(corner2);
                bool corner3InViewport = _camera.InViewport(corner3);
                bool corner4InViewport = _camera.InViewport(corner4);

                if (!corner1InViewport && !corner2InViewport && !corner3InViewport && !corner4InViewport)
                {
                    continue;
                }

                chunksInView.Add(chunkPosition);
            }

            HashSet<Vector2Int> chunksToRender = new(chunksInView);

            // foreach visible chunk, render its adjacent chunks
            foreach (Vector2Int chunkPosition in chunksToRender.ToArray())
            {
                chunksToRender.Add(chunkPosition + Vector2Int.up);
                chunksToRender.Add(chunkPosition + Vector2Int.down);
                chunksToRender.Add(chunkPosition + Vector2Int.left);
                chunksToRender.Add(chunkPosition + Vector2Int.right);
                chunksToRender.Add(chunkPosition + Vector2Int.up + Vector2Int.left);
                chunksToRender.Add(chunkPosition + Vector2Int.up + Vector2Int.right);
                chunksToRender.Add(chunkPosition + Vector2Int.down + Vector2Int.left);
                chunksToRender.Add(chunkPosition + Vector2Int.down + Vector2Int.right);
            }
            
            int nVisibleChunks = chunksToRender.Count;

            chunks ??= new List<MapChunk>();

            HashSet<Vector2Int> toBeSkipped = new();
            foreach (MapChunk chunk in chunks)
            {
                if (chunksToRender.Contains(chunk.state.position))
                {
                    toBeSkipped.Add(chunk.state.position);
                    chunksToRender.Remove(chunk.state.position);

                    chunk.gameObject.SetActive(true);
                    chunk.Set(map, chunk.state.position, chunksInView.Contains(chunk.state.position));
                }
            }

            for (int i = chunks.Count; i < nVisibleChunks; i++)
            {
                MapChunk newChunk = Instantiate(map.runtimeConfig.baseChunk, transform);
                chunks.Add(newChunk);

                Vector2Int positionForCurrentChunk = chunksToRender.First();
                chunksToRender.Remove(positionForCurrentChunk);
                toBeSkipped.Add(positionForCurrentChunk);

                newChunk.Set(map, positionForCurrentChunk, chunksInView.Contains(positionForCurrentChunk));

                yield return null;
            }

            if (chunksToRender.Any())
            {
                foreach (MapChunk chunk in chunks)
                {
                    if (toBeSkipped.Contains(chunk.state.position))
                    {
                        continue;
                    }
                    
                    chunk.gameObject.SetActive(true);

                    if (chunksToRender.Count == 0)
                    {
                        chunk.gameObject.SetActive(false);
                        continue;
                    }

                    Vector2Int positionForCurrentChunk = chunksToRender.First();
                    chunksToRender.Remove(positionForCurrentChunk);

                    chunk.Set(map, positionForCurrentChunk, chunksInView.Contains(positionForCurrentChunk));

                    yield return null;
                }
            }

            _updateCoroutine = null;
            Ready = true;
        }
    }
}
