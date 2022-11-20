using System;
using Map;
using Map.Tile;
using Utils;

namespace State
{
    [Serializable]
    public class MapState
    {
        public MapConfig config;
        public TileConfig[] tiles;

        public TileConfig GetTileAt(int x, int y)
        {
            return tiles[MyMath.GetIndex(x, y, config.mapSize)];
        }
    }
}
