using System;

namespace Map.Tile
{
    public enum TileType
    {
        None = 0,
        Grass = 1,
        Gravel = 2,
    }

    [Flags]
    public enum TileTypeMask
    {
        None = 0,
        Grass = 1,
        Gravel = 2,
    }

    public static class TileTypeExtensions
    {
        public static bool Check(this TileTypeMask mask, TileType type)
        {
            if (type == TileType.None)
            {
                return false;
            }

            TileTypeMask flag = (TileTypeMask)(1<<((int)type - 1));
            return mask.HasFlag(flag);
        }
    }
}
