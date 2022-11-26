using System;

namespace Map.Tile
{
    [Serializable]
    public class TileConfig
    {
        public TileType type;
        public TileResourceType tileResource;

        public static TileConfig Empty => new();
    }
}