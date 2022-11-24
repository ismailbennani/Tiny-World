using System;
using Resource;

namespace Map.Tile
{
    [Serializable]
    public class TileConfig
    {
        public TileType type;
        public ResourceType resource;
        
        public static TileConfig Empty => new();
    }
}