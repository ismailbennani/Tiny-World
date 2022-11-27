using Map.Chunk;
using Map.Tile;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "Custom/Map runtime config")]
    public class MapRuntimeConfig: ScriptableObject
    {
        public MapChunk baseChunk;
        public MapTile baseTile;

        [Header("Map rendering")]
        [Tooltip("Number of chunks to load in each direction: (1, 1) means that a (3,3) square is loaded at all time")]
        public Vector2Int maxChunkRange = 2 * Vector2Int.one;
        
        [Tooltip("Scale applied to each tile")]
        public Vector2 tileSize = 4 * Vector2.one;

        [Tooltip("Scale applied to the resources prefabs")]
        public float resourceScale = 4;
    }
}
