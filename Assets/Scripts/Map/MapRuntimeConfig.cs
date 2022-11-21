using System;
using Map.Chunk;
using Map.Tile;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class MapRuntimeConfig
    {
        public MapChunk baseChunk;
        public MapTile baseTile;

        [Header("Map rendering")]
        [Tooltip("Number of chunks to load in each direction: (1, 1) means that a (3,3) square is loaded at all time")]
        public Vector2Int chunkRange = 2 * Vector2Int.one;
        
        [Tooltip("Scale applied to each tile")]
        public Vector2 tileSize = 4 * Vector2.one;

        [Tooltip("Scale applied to the resources prefabs")]
        public float resourceScale = 4;
    }
}
