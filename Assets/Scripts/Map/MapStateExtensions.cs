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
        
        public static Vector3 GetTileCenterPosition(this MapState mapState, Vector2Int tile)
        {
            return mapState.GetTileCenterPosition(tile.x, tile.y);
        }
    }
}
