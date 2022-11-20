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
        public Vector2 tileSize = Vector2.one;
        public Vector2 gap = Vector2.zero;
        public Vector2Int mapSize = Vector2Int.one;
        public float resourceScale = 1;
    }
}