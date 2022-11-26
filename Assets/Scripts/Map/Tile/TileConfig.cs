using System;
using Resource;

namespace Map.Tile
{
    [Serializable]
    public class TileConfig
    {
        public TileType type;
        public ResourceType resource;
        public int nPlatformVariants = 1;
        public int nResourceVariants = 1;

        public static TileConfig Empty => new();
    }
}