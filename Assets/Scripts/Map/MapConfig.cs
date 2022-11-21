using System;
using Map.Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    [Serializable]
    public class MapConfig
    {
        public MapTile baseTile;

        [Header("Map config")]
        public int seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        public TileWithWeight[] tiles;
        public Vector2 tileSize = Vector2.one;
        public Vector2 gap = Vector2.zero;
        public Vector2Int mapSize = Vector2Int.one;
        public float resourceScale = 1;
    }
}