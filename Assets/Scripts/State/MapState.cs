using System;
using Map;
using Map.Tile;
using UnityEngine;
using Utils;

namespace State
{
    [Serializable]
    public class MapState
    {
        public MapConfig config;
        public TileConfig[] tiles;

        public Vector3 mapOrigin;

        public TileConfig GetTileConfig(int x, int y)
        {
            return tiles[MyMath.GetIndex(x, y, config.mapSize)];
        }

        public Vector2Int GetTileAt(Vector3 position)
        {
            Vector2 halfTile = config.tileSize / 2;
            Vector3 realOrigin = mapOrigin - new Vector3(halfTile.x, 0, halfTile.y);

            Vector3 delta = position - realOrigin;
            Vector2 tileAndGap = config.tileSize + config.gap;

            return new Vector2Int(Mathf.FloorToInt(delta.x / tileAndGap.x), Mathf.FloorToInt(delta.z / tileAndGap.y));
        }
    }
}
