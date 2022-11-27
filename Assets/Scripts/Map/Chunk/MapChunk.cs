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
        public ChunkState state;

        [SerializeField]
        private MapTile baseTile;

        private Coroutine _spawnCoroutine;
        private bool _chunkFullySpawned;
        private bool _hidden;

        public void Set(MapState map, Vector2Int chunk, bool urgent)
        {
            if (!_hidden && state?.position == chunk && (_chunkFullySpawned || !urgent))
            {
                return;
            }

            state = map.GetChunk(chunk);
            baseTile = map.runtimeConfig.baseTile;

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
                UpdateChunkImmediately(map, chunk);
            }
            else
            {
                _spawnCoroutine = StartCoroutine(UpdateChunkCoroutine(map, chunk));
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

        private void UpdateChunkImmediately(MapState map, Vector2Int chunk)
        {
            IEnumerator coroutine = UpdateChunkCoroutine(map, chunk);
            while (coroutine.MoveNext())
            {
            }

            _chunkFullySpawned = true;
        }

        private IEnumerator UpdateChunkCoroutine(MapState map, Vector2Int chunk)
        {
            int nTiles = state.size.x * state.size.y;

            for (int i = tiles.Count; i < nTiles; i++)
            {
                MapTile newTile = Instantiate(baseTile, transform);
                tiles.Add(newTile);

                yield return null;
            }

            Vector2Int playerPosition = GameStateManager.Current.player.playerTile;
            IOrderedEnumerable<TileState> sortedTilesToUpdate = state.tiles.OrderBy(t => Vector2.Distance(t.position, playerPosition));

            int index = 0;
            foreach (TileState tile in sortedTilesToUpdate)
            {
                MapTile mapTile = tiles[index];
                mapTile.gameObject.SetActive(true);

                mapTile.UpdateState(tile);

                mapTile.transform.position = map.GetTileCenterPosition(tile.position);

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
