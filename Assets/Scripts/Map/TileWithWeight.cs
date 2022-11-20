using System;
using Map.Tile;

namespace Map
{
    [Serializable]
    public class TileWithWeight
    {
        public TileConfig tileConfig;
        public float weight = 1;
    }
}
