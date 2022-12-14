using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map.Tile;
using UnityEngine;

namespace Map.Chunk
{
    public class MapChunk : MonoBehaviour
    {
        public List<MapTile> tiles;
        public Vector2Int position;

        [SerializeField]
        private MapTile baseTile;

        private Coroutine _spawnCoroutine;
        private bool _chunkFullySpawned;
        private bool _hidden;

        public void Set(GameState gameState, Vector2Int newPosition, bool urgent)
        {
            if (!_hidden && position == newPosition && (_chunkFullySpawned || !urgent))
            {
                return;
            }

            position = newPosition;
            ChunkState state = gameState.map.GetChunk(newPosition);
            baseTile = gameState.map.runtimeConfig.baseTile;

            tiles ??= new List<MapTile>();

            _chunkFullySpawned = false;

            if (_hidden)
            {
                gameObject.SetActive(true);
                _hidden = false;
            }

            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }

            if (urgent)
            {
                UpdateChunkImmediately(gameState, state);
            }
            else
            {
                _spawnCoroutine = StartCoroutine(UpdateChunkCoroutine(gameState, state));
            }
        }

        public void Hide()
        {
            if (_hidden)
            {
                return;
            }

            gameObject.SetActive(false);
            _hidden = true;
        }

        private void UpdateChunkImmediately(GameState gameState, ChunkState state)
        {
            IEnumerator coroutine = UpdateChunkCoroutine(gameState, state);
            while (coroutine.MoveNext())
            {
            }

            _chunkFullySpawned = true;
        }

        private IEnumerator UpdateChunkCoroutine(GameState gameState, ChunkState state)
        {
            int nTiles = state.size.x * state.size.y;

            for (int i = tiles.Count; i < nTiles; i++)
            {
                MapTile newTile = Instantiate(baseTile, transform);
                newTile.gameState = gameState;
                tiles.Add(newTile);

                yield return null;
            }

            Vector2Int playerPosition = GameStateManager.Current.player.tile;
            IOrderedEnumerable<TileState> sortedTilesToUpdate = state.tiles.OrderBy(t => Vector2.Distance(t.position, playerPosition));

            int index = 0;
            foreach (TileState tile in sortedTilesToUpdate)
            {
                MapTile mapTile = tiles[index];
                mapTile.gameObject.SetActive(true);

                mapTile.UpdateState(tile);

                mapTile.transform.position = gameState.map.GetTileCenterPosition(tile.position);

                index++;
                yield return null;
            }

            for (int i = nTiles; i < tiles.Count; i++)
            {
                tiles[i].gameObject.SetActive(false);
            }

            _spawnCoroutine = null;
            _chunkFullySpawned = true;
        }
    }
}
