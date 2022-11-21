using Map.Tile;
using UnityEngine;

namespace Map.Generation
{
    public static class MapGeneratorExtensions
    {
        public static TileConfig GenerateTile(this IMapGenerator mapGenerator, Vector2Int tile)
        {
            return mapGenerator.GenerateTile(tile.x, tile.y);
        }
    }
}
