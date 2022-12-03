using System;
using System.Collections.Generic;
using System.Linq;
using MackySoft.Choice;
using Map.Tile;

namespace Map.Generation
{
    public class RandomMapGenerator : IMapGenerator
    {
        private Random _random;
        private IWeightedSelector<TileWithWeight> _tileSelector;
        private Dictionary<TileType, IWeightedSelector<ResourceWithWeight>> _resourcesSelectors;
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
            _resourcesSelectors = config.tiles.ToDictionary(
                t => t.type,
                t => _config.resources.Where(r => t.type.IsCompatibleWith(r.expectedTile)).ToWeightedSelector(r => r.weight, WeightedSelectMethod.Alias)
            );
        }

        public TileConfig GenerateTile(int x, int y)
        {
            TileWithWeight tile = _tileSelector.SelectItem((float)_random.NextDouble());
            ResourceWithWeight resource = _resourcesSelectors[tile.type].SelectItem((float)_random.NextDouble());

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
