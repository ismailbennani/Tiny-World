using System;
using System.Linq;

namespace Map.Tile
{
    public static class TileTypeExtensions
    {
        public static bool IsCompatibleWith(this TileType type, TileType requiredType)
        {
            return Enum.GetValues(typeof(TileType)).Cast<TileType>().All(value => !requiredType.HasFlag(value) || type.HasFlag(value));
        }
    }
}
