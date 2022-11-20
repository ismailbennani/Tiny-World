using System;
using Map.Tile;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class MapConfig
    {
        public MapTile baseTile;

        [Header("Map config")]
        public TileWithWeight[] tiles;
        public Vector2 tileSize;
        public Vector2 gap;
        public Vector2Int mapSize;
    }
}