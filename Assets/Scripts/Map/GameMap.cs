using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
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
        private List<GameItem> items;

        [SerializeField]
        private Vector2Int assumedPlayerChunk;

        private Coroutine _updateCoroutine;
        private Camera _camera;

        private void OnEnable()
        {
            Instance = this;
        }

        public void Initialize()
        {
            Ready = false;

            _updateCoroutine = StartCoroutine(UpdateChunks(GameStateManager.Current));
        }

        void Update()
        {
            if (_updateCoroutine == null)
            {
                _updateCoroutine = StartCoroutine(UpdateChunks(GameStateManager.Current));
            }
        }

        public void SpawnItem(Item item, Vector3 position)
        {
            GameState gameState = GameStateManager.Current;

            ItemState itemState = new(item)
            {
                position = position + new Vector3(Random.Range(-0.5f, 0.5f), 1, Random.Range(-0.5f, 0.5f))
            };
            gameState.map.AddItem(itemState);
        }

        public MapTile GetTile(TileState tile)
        {
            if (chunks == null)
            {
                return null;
            }

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
            MapChunk chunk = chunks.SingleOrDefault(c => c.position == chunkPosition);
            if (chunk == null)
            {
                return null;
            }

            return chunk.tiles.SingleOrDefault(t => t.hasPosition && t.tilePosition == tile.position);
        }

        private IEnumerator UpdateChunks(GameState gameState)
        {
            assumedPlayerChunk = gameState.player.chunk;

            GetChunksToRender(gameState.map, out HashSet<Vector2Int> chunksInView, out HashSet<Vector2Int> chunksToRender);

            IEnumerator updateTilesCoroutine = UpdateTiles(gameState, chunksToRender, chunksInView);
            while (updateTilesCoroutine.MoveNext())
            {
                yield return null;
            }

            IEnumerator updateItemsCoroutine = UpdateItems(gameState, chunksInView);
            while (updateItemsCoroutine.MoveNext())
            {
                yield return null;
            }

            _updateCoroutine = null;
            Ready = true;
        }

        private IEnumerator UpdateTiles(GameState state, HashSet<Vector2Int> chunksToRender, HashSet<Vector2Int> chunksInView)
        {
            int nVisibleChunks = chunksToRender.Count;
            chunks ??= new List<MapChunk>();

            HashSet<Vector2Int> toBeSkipped = new();
            foreach (MapChunk chunk in chunks)
            {
                if (chunksToRender.Contains(chunk.position))
                {
                    toBeSkipped.Add(chunk.position);
                    chunksToRender.Remove(chunk.position);

                    chunk.Set(state, chunk.position, chunksInView.Contains(chunk.position));
                }
            }

            for (int i = chunks.Count; i < nVisibleChunks; i++)
            {
                MapChunk newChunk = Instantiate(state.map.runtimeConfig.baseChunk, transform);
                chunks.Add(newChunk);

                Vector2Int positionForCurrentChunk = chunksToRender.First();
                chunksToRender.Remove(positionForCurrentChunk);
                toBeSkipped.Add(positionForCurrentChunk);

                newChunk.gameObject.SetActive(true);
                newChunk.Set(state, positionForCurrentChunk, chunksInView.Contains(positionForCurrentChunk));

                yield return null;
            }

            foreach (MapChunk chunk in chunks)
            {
                if (toBeSkipped.Contains(chunk.position))
                {
                    continue;
                }

                if (chunksToRender.Count == 0)
                {
                    chunk.Hide();
                }
                else
                {
                    Vector2Int positionForCurrentChunk = chunksToRender.First();
                    chunksToRender.Remove(positionForCurrentChunk);

                    chunk.Set(state, positionForCurrentChunk, chunksInView.Contains(positionForCurrentChunk));

                    yield return null;
                }
            }
        }

        private IEnumerator UpdateItems(GameState gameState, IEnumerable<Vector2Int> chunksInView)
        {
            Dictionary<string, ItemState> newItems = chunksInView.SelectMany(gameState.map.GetItemsInChunk).ToDictionary(i => i.guid, i => i);

            HashSet<string> itemsToDisplay = newItems.Keys.ToHashSet();
            int nItems = itemsToDisplay.Count;

            items ??= new List<GameItem>();

            HashSet<string> toBeSkipped = new();
            foreach (GameItem item in items.Where(item => itemsToDisplay.Contains(item.guid)))
            {
                toBeSkipped.Add(item.guid);
                itemsToDisplay.Remove(item.guid);

                item.Set(newItems[item.guid]);
            }

            for (int i = items.Count; i < nItems; i++)
            {
                GameItem newItem = Instantiate(gameState.itemsConfig.baseItem, transform);
                items.Add(newItem);

                string newItemGuid = itemsToDisplay.First();
                itemsToDisplay.Remove(newItemGuid);
                toBeSkipped.Add(newItemGuid);

                newItem.gameObject.SetActive(true);

                ItemState itemStateForNewItem = newItems[newItemGuid];
                newItem.Set(itemStateForNewItem);

                yield return null;
            }

            foreach (GameItem item in items.Where(item => !toBeSkipped.Contains(item.guid)))
            {
                if (itemsToDisplay.Count == 0)
                {
                    item.Hide();
                }
                else
                {
                    string itemGuid = itemsToDisplay.First();
                    itemsToDisplay.Remove(itemGuid);

                    ItemState itemState = newItems[itemGuid];
                    item.Set(itemState);

                    yield return null;
                }
            }
        }

        private void GetChunksToRender(MapState map, out HashSet<Vector2Int> chunksInView, out HashSet<Vector2Int> chunksToRender)
        {
            chunksInView = new HashSet<Vector2Int>
            {
                assumedPlayerChunk
            };

            if (!_camera)
            {
                _camera = Camera.main;
            }

            for (int x = -map.runtimeConfig.maxChunkRange.x; x <= map.runtimeConfig.maxChunkRange.x; x++)
            for (int y = -map.runtimeConfig.maxChunkRange.y; y <= map.runtimeConfig.maxChunkRange.y; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y) + assumedPlayerChunk;

                Rect chunkRect = map.GetChunkRect(chunkPosition);

                Vector3 corner1 = new(chunkRect.xMin, 0, chunkRect.yMin);
                Vector3 corner2 = new(chunkRect.xMax, 0, chunkRect.yMin);
                Vector3 corner3 = new(chunkRect.xMax, 0, chunkRect.yMax);
                Vector3 corner4 = new(chunkRect.xMin, 0, chunkRect.yMax);

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

            chunksToRender = new HashSet<Vector2Int>(chunksInView);

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
        }
    }
}
