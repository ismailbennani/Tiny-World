using System;
using Map.Tile;
using UnityEngine;
using Utils;

namespace Map
{
    [Serializable]
    public class MapState
    {
        public MapConfig config;
        public TileState[] tiles;

        public Vector3 mapOrigin;

        public TileState GetTile(int x, int y)
        {
            return tiles[MyMath.GetIndex(x, y, config.mapSize)];
        }
        
        public TileState GetTile(Vector2Int tile)
        {
            return GetTile(tile.x, tile.y);
        }

        public Vector2Int GetTileAt(Vector3 position)
        {
            Vector2 halfTile = config.tileSize / 2;
            Vector3 realOrigin = mapOrigin - new Vector3(halfTile.x, 0, halfTile.y);

            Vector3 delta = position - realOrigin;
            Vector2 tileAndGap = config.tileSize + config.gap;

            return new Vector2Int(Mathf.FloorToInt(delta.x / tileAndGap.x), Mathf.FloorToInt(delta.z / tileAndGap.y));
        }

        public Vector3 GetTileCenterPosition(Vector2Int tile)
        {
            return GetTileCenterPosition(tile.x, tile.y);
        }
        
        public Vector3 GetTileCenterPosition(int x, int y)
        {
            return new Vector3(x * (config.tileSize.x + config.gap.x), 0, y * (config.tileSize.y + config.gap.y));
        }

        public Vector3 GetTileCenterPosition(int index)
        {
            return GetTileCenterPosition(tiles[index].position);
        }
    }
}
