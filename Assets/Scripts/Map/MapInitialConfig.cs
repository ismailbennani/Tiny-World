using System;
using Map.Generation;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class MapInitialConfig
    {
        [Header("Map generation")]
        [Tooltip("Chunk size in tiles")]
        public Vector2Int chunkSize = 5 * Vector2Int.one;

        [Tooltip("Number of chunks to generate initially")]
        public Vector2Int initialChunks = 5 * Vector2Int.one;

        [Tooltip("Random seed")]
        public int seed;

        [Tooltip("Tile configs to sample from")]
        public TileWithWeight[] tiles;

        [Tooltip("What algorithm to use to generate the map")]
        public MapGenerationAlgorithm mapGenerationAlgorithm;
    }
}
