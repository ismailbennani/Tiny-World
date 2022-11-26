using System.Collections.Generic;
using Items;
using Map.Chunk;
using Map.Tile;
using UnityEngine;

namespace Map
{
    public static class MapStateExtensions
    {
        public static TileState GetTile(this MapState mapState, Vector2Int tilePosition)
        {
            return mapState.GetTile(tilePosition.x, tilePosition.y);
        }
        
        public static ChunkState GetChunk(this MapState mapState, Vector2Int chunkPosition)
        {
            return mapState.GetChunk(chunkPosition.x, chunkPosition.y);
        }

        public static Vector2Int GetTilePositionAt(this MapState mapState, Vector3 worldPosition)
        {
            (int x, int y) = mapState.GetTilePosition(worldPosition);
            return new Vector2Int(x, y);
        }

        public static Vector2Int GetChunkPositionAt(this MapState mapState, Vector2Int tilePosition)
        {
            (int x, int y) = mapState.GetChunkPosition(tilePosition.x, tilePosition.y);
            return new Vector2Int(x, y);
        }

        public static Vector2Int GetChunkPositionAt(this MapState mapState, Vector3 worldPosition)
        {
            (int x, int y) = mapState.GetChunkPosition(worldPosition);
            return new Vector2Int(x, y);
        }

        public static Vector3 GetTileCenterPosition(this MapState mapState, Vector2Int tile)
        {
            Vector2 center = mapState.GetTileRect(tile.x, tile.y).center;
            return new Vector3(center.x, 0, center.y);
        }

        public static Rect GetTileRect(this MapState mapState, Vector2Int tilePosition)
        {
            return mapState.GetTileRect(tilePosition.x, tilePosition.y);
        }

        public static Rect GetChunkRect(this MapState mapState, Vector2Int chunkPosition)
        {
            return mapState.GetChunkRect(chunkPosition.x, chunkPosition.y);
        }

        public static IEnumerable<ItemState> GetItemsInChunk(this MapState mapState, Vector2Int chunkPosition)
        {
            return mapState.GetItemsInChunk(chunkPosition.x, chunkPosition.y);
        }
    }
}
