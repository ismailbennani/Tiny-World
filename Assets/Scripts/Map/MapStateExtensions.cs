using Map.Chunk;
using Map.Tile;
using UnityEngine;

namespace Map
{
    public static class MapStateExtensions
    {
        public static TileState GetTile(this MapState mapState, Vector2Int tile)
        {
            return mapState.GetTile(tile.x, tile.y);
        }
        
        public static ChunkState GetChunk(this MapState mapState, Vector2Int tile)
        {
            return mapState.GetChunk(tile.x, tile.y);
        }
        
        public static Vector3 GetTileCenterPosition(this MapState mapState, Vector2Int tile)
        {
            return mapState.GetTileCenterPosition(tile.x, tile.y);
        }
    }
}
