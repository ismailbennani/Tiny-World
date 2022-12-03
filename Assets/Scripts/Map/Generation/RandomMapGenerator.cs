using System;
using MackySoft.Choice;
using Map.Tile;

namespace Map.Generation
{
    public class RandomMapGenerator : IMapGenerator
    {
        private Random _random;
        private IWeightedSelector<TileWithWeight> _tileSelector;
        private MapInitialConfig _config;

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
            _config = config;
            _tileSelector = config.tiles.ToWeightedSelector(t => t.weight, WeightedSelectMethod.Alias);
        }

        public TileConfig GenerateTile(int x, int y)
        {
            TileWithWeight tile = _tileSelector.SelectItem((float)_random.NextDouble());
            ResourceWithWeight resource = _config.resources.ToWeightedSelector(r => r.weight).SelectItem((float)_random.NextDouble());
            
            return new TileConfig
            {
                type = tile.type,
                tileResource = resource.resource,
                lootTable = resource.lootTable,
                nLoots = resource.nLoots
            };
        }
    }
}
