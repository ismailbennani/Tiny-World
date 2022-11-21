using Map.Tile;
using UnityEngine;
using Utils;

namespace Map.Generation
{
    public static class MapGeneratorExtensions
    {
        public static TileConfig GenerateTile(this IMapGenerator mapGenerator, Vector2Int tile)
        {
            return mapGenerator.GenerateTile(tile.x, tile.y);
        }
        
        public static TileConfig[] GenerateChunk(this IMapGenerator mapGenerator, Vector2Int at, Vector2Int size)
        {
            TileConfig[] result = new TileConfig[size.x * size.y];

            for (int index = 0; index < result.Length; index++)
            {
                (int x, int y) = MyMath.GetCoords(index, size);
                result[index] = mapGenerator.GenerateTile(at.x + x, at.y + y);
            }

            return result;
        }
    }
}
