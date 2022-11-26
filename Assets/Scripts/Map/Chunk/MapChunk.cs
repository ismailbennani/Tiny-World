using System.Collections;
using System.Collections.Generic;
using Map.Tile;
using UnityEngine;
using Utils;

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

        public void Set(MapState map, Vector2Int chunk, bool urgent)
        {
            if (state?.position == chunk && (_chunkFullySpawned || !urgent))
            {
                return;
            }

            state = map.GetChunk(chunk);
            baseTile = map.runtimeConfig.baseTile;

            tiles ??= new List<MapTile>();

            _chunkFullySpawned = false;
            
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            
            if (urgent)
            {
                SpawnChunkImmediately(map, chunk);
            }
            else
            {
                _spawnCoroutine = StartCoroutine(SpawnChunkCoroutine(map, chunk));
            }
        }

        private void SpawnChunkImmediately(MapState map, Vector2Int chunk)
        {
            IEnumerator coroutine = SpawnChunkCoroutine(map, chunk);
            while (coroutine.MoveNext())
            {    
            }

            _chunkFullySpawned = true;
        }

        private IEnumerator SpawnChunkCoroutine(MapState map, Vector2Int chunk)
        {
            int nTiles = state.size.x * state.size.y;

            for (int i = tiles.Count; i < nTiles; i++)
            {
                MapTile newTile = Instantiate(baseTile, transform);
                tiles.Add(newTile);

                yield return null;
            }

            for (int index = 0; index < nTiles; index++)
            {
                MapTile tile = tiles[index];
                tile.gameObject.SetActive(true);

                (int x, int y) = MyMath.GetCoords(index, state.size);
                TileState tileConfig = state.GetTile(x, y);

                tile.Spawn(tileConfig);

                Vector3 position = map.GetTileCenterPosition(x + state.gridPosition.x, y + state.gridPosition.y);
                tile.transform.position = position;

                yield return null;
            }

            for (int index = nTiles; index < tiles.Count; index++)
            {
                tiles[index].gameObject.SetActive(false);
            }

            _spawnCoroutine = null;
            _chunkFullySpawned = true;
        }
    }
}
