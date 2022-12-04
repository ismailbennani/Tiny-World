using System;
using Items;
using Map.Generation;
using Map.Tile;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "Custom/Map initial config")]
    public class MapInitialConfig : ScriptableObject
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
        
        [Tooltip("Resource configs to sample from")]
        public ResourceWithWeight[] resources;

        [Tooltip("What algorithm to use to generate the map")]
        public MapGenerationAlgorithm mapGenerationAlgorithm;
        public Vector2 scale;
        public Vector2 offset;
    }

    [Serializable]
    public class TileWithWeight
    {
        public TileType type;
        public float weight = 1;
    }

    [Serializable]
    public class ResourceWithWeight
    {
        public TileResourceType resource;
        public float weight = 1;

        [Header("Constraints")]
        public TileTypeMask expectedTile;
        
        [Header("Loot")]
        public LootTable lootTable;
        public Vector2Int nLoots;
    }
}
