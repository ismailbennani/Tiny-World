using System;
using MackySoft.Choice;
using Map.Tile;

namespace Map.Generation
{
    public class RandomMapGenerator : IMapGenerator
    {
        private Random _random;
        private IWeightedSelector<TileWithWeight> _selector;

        public void SetConfiguration(MapInitialConfig config)
        {
            if (config == null)
            {
                throw new InvalidOperationException("Please call SetConfiguration first");
            }

            if (config.tiles.Length == 0)
            {
                throw new InvalidOperationException("Did not expect tiles to be empty");
            }
            
            _random = new Random(config.seed);
            _selector = config.tiles.ToWeightedSelector(t => t.weight, WeightedSelectMethod.Alias);
        }

        public TileConfig GenerateTile(int x, int y)
        {
            return _selector.SelectItem((float)_random.NextDouble()).tileConfig;
        }
    }
}
