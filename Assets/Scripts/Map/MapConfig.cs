using Map.Tile;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "Custom/Map config")]
    public class MapConfig: ScriptableObject
    {
        public MapTile baseTile;

        [Header("Map config")]
        public TileWithWeight[] tiles;
        public Vector2 tileSize;
        public Vector2 gap;
        public Vector2Int mapSize;
    }
}