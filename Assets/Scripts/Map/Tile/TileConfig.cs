using System;
using Items;
using UnityEngine;

namespace Map.Tile
{
    [Serializable]
    public class TileConfig
    {
        public TileType type;
        public TileResourceType tileResource;
        
        [Header("Loot")]
        public LootTable lootTable;
        public Vector2Int nLoots = Vector2Int.one;
      
        public static TileConfig Empty => new();
    }
}