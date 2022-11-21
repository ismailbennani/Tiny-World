using Map.Tile;

namespace Map.Generation
{
    public interface IMapGenerator
    {
        void SetConfiguration(MapConfig config);
        TileConfig GenerateTile(int x, int y);
    }
}
