using System;
using System.Collections.Generic;
using System.Linq;
using Items;
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

        public List<ChunkState> chunks;
        public List<ItemState> items;

        private IMapGenerator _mapGenerator;

        public MapState(GameConfig gameConfig)
        {
            initialConfig = gameConfig.mapInitialConfig;
            runtimeConfig = gameConfig.mapRuntimeConfig;
            chunks = new List<ChunkState>();
        }

        public TileState GetTile(int x, int y)
        {
            (int chunkX, int chunkY) = GetChunkPosition(x, y);

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

        public void AddItem(ItemState itemState)
        {
            items.Add(itemState);
        }

        public void RemoveItem(ItemState itemState)
        {
            items.Remove(itemState);
        }

        public IEnumerable<ItemState> GetItemsInChunk(int chunkX, int chunkY)
        {
            Rect chunkRect = GetChunkRect(chunkX, chunkY);
            return items.Where(i => chunkRect.Contains(new Vector2(i.position.x, i.position.z)));
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

        public (int, int) GetTilePosition(Vector3 worldPosition)
        {
            return (Mathf.FloorToInt(worldPosition.x / runtimeConfig.tileSize.x), Mathf.FloorToInt(worldPosition.z / runtimeConfig.tileSize.y));
        }

        public (int, int) GetChunkPosition(Vector3 worldPosition)
        {
            (int x, int y) = GetTilePosition(worldPosition);

            return GetChunkPosition(x, y);
        }

        public (int, int) GetChunkPosition(int tileX, int tileY)
        {
            int chunkX = (tileX < 0 ? tileX + 1 : tileX) / initialConfig.chunkSize.x;
            if (tileX < 0)
            {
                chunkX--;
            }

            int chunkY = (tileY < 0 ? tileY + 1 : tileY) / initialConfig.chunkSize.y;
            if (tileY < 0)
            {
                chunkY--;
            }

            return (chunkX, chunkY);
        }

        public Rect GetTileRect(int x, int y)
        {
            Vector2 start = new(x * runtimeConfig.tileSize.x, y * runtimeConfig.tileSize.y);
            
            return new Rect(start, runtimeConfig.tileSize);
        }

        public Rect GetChunkRect(int chunkX, int chunkY)
        {
            Vector2 size = new(initialConfig.chunkSize.x * runtimeConfig.tileSize.x, initialConfig.chunkSize.y * runtimeConfig.tileSize.y);
            Vector2 start = new(chunkX * size.x, chunkY * size.y);

            return new Rect(start, size);
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
