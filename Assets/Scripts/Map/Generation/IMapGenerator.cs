using Map.Tile;

namespace Map.Generation
{
    public interface IMapGenerator
    {
        void SetConfiguration(MapInitialConfig config);
        TileConfig GenerateTile(int x, int y);
    }
}
