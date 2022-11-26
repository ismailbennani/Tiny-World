using System;
using System.Collections.Generic;
using System.Linq;
using Map.Chunk;
using Map.Generation;
using Map.Tile;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class MapState
    {
        public bool IsValid => initialConfig != null;

        public MapInitialConfig initialConfig;
        public MapRuntimeConfig runtimeConfig;

        [SerializeField]
        public List<ChunkState> chunks;

        private IMapGenerator _mapGenerator;

        public MapState(GameConfig gameConfig)
        {
            initialConfig = gameConfig.mapInitialConfig;
            runtimeConfig = gameConfig.mapRuntimeConfig;
            chunks = new List<ChunkState>();
        }

        public TileState GetTile(int x, int y)
        {
            int chunkX = (x < 0 ? x + 1 : x) / initialConfig.chunkSize.x;
            if (x < 0)
            {
                chunkX--;
            }

            int chunkY = (y < 0 ? y + 1 : y) / initialConfig.chunkSize.y;
            if (y < 0)
            {
                chunkY--;
            }

            int tileX = x - chunkX * initialConfig.chunkSize.x;
            int tileY = y - chunkY * initialConfig.chunkSize.y;

            ChunkState chunk = GetChunk(chunkX, chunkY);
            return chunk.GetTile(tileX, tileY);
        }

        public void PrepareChunk(int chunkX, int chunkY)
        {
            // compute chunks if they don't exist already
            GetChunk(chunkX, chunkY);
        }

        public ChunkState GetChunk(int chunkX, int chunkY)
        {
            return chunks.SingleOrDefault(c => c.position.x == chunkX && c.position.y == chunkY) ?? GenerateChunk(chunkX, chunkY);
        }

        private ChunkState GenerateChunk(int chunkX, int chunkY)
        {
            if (_mapGenerator == null)
            {
                _mapGenerator = GetMapGenerator(initialConfig.mapGenerationAlgorithm);
                _mapGenerator.SetConfiguration(initialConfig);
            }

            Vector2Int chunkPosition = new(chunkX, chunkY);
            Vector2Int chunkGridPosition = new(chunkX * initialConfig.chunkSize.x, chunkY * initialConfig.chunkSize.y);

            List<TileState> tiles = new();
            for (int x = 0; x < initialConfig.chunkSize.x; x++)
            for (int y = 0; y < initialConfig.chunkSize.y; y++)
            {
                Vector2Int tilePosition = chunkGridPosition + new Vector2Int(x, y);
                TileConfig tileConfig = _mapGenerator.GenerateTile(tilePosition);

                tiles.Add(new TileState(tileConfig, tilePosition));
            }

            ChunkState newChunk = new()
            {
                position = chunkPosition,
                gridPosition = chunkGridPosition,
                size = initialConfig.chunkSize,
                tiles = tiles.ToArray(),
            };

            chunks.Add(newChunk);

            return newChunk;
        }

        public Vector2Int GetTilePositionAt(Vector3 position)
        {
            Vector2 halfTile = runtimeConfig.tileSize / 2;
            Vector3 realOrigin = new(-halfTile.x, 0, -halfTile.y);

            Vector3 delta = position - realOrigin;
            Vector2 tileAndGap = runtimeConfig.tileSize;

            return new Vector2Int(Mathf.FloorToInt(delta.x / tileAndGap.x), Mathf.FloorToInt(delta.z / tileAndGap.y));
        }

        public Vector2Int GetChunkPositionAt(Vector3 position)
        {
            Vector2Int tile = GetTilePositionAt(position);

            return GetChunkPositionAt(tile);
        }

        public Vector2Int GetChunkPositionAt(Vector2Int tilePosition)
        {
            int x = tilePosition.x / initialConfig.chunkSize.x;
            if (tilePosition.x < 0)
            {
                x--;
            }

            int y = tilePosition.y / initialConfig.chunkSize.y;
            if (tilePosition.y < 0)
            {
                y--;
            }

            return new Vector2Int(x, y);
        }

        public Vector3 GetTileCenterPosition(int x, int y)
        {
            return new Vector3(x * runtimeConfig.tileSize.x, 0, y * runtimeConfig.tileSize.y);
        }

        private IMapGenerator GetMapGenerator(MapGenerationAlgorithm algorithm)
        {
            return algorithm switch
            {
                MapGenerationAlgorithm.Random => new RandomMapGenerator(),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
            };
        }
    }
}
