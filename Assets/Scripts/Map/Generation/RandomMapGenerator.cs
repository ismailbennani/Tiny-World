using System;
using System.Collections.Generic;
using System.Linq;
using MackySoft.Choice;
using Map.Generation.RandomGenerator;
using Map.Tile;
using UnityEngine;

namespace Map.Generation
{
    public class RandomMapGenerator : IMapGenerator
    {
        private IRandomGenerator _randomTile;
        private IRandomGenerator _randomResource;
        private IWeightedSelector<TileWithWeight> _tileSelector;
        private Dictionary<TileType, IWeightedSelector<ResourceWithWeight>> _resourcesSelectors;

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

            var resourcesPerTileType = config.tiles.Select(t => new { Type = t.type, Resources = config.resources.Where(r => r.expectedTile.Check(t.type)) })
                .Where(tr => tr.Resources.Any()).ToList();

            Debug.Log(
                $"[MAP GENERATION] Resources per tile type:{Environment.NewLine}"
                + string.Join(
                    Environment.NewLine,
                    config.tiles.Select(
                        t => $"{t.type}: "
                             + string.Join(
                                 ", ",
                                 resourcesPerTileType.SingleOrDefault(r => r.Type == t.type)?.Resources.Select(r => $"{r.resource} ({r.weight})")
                                 ?? Enumerable.Empty<string>()
                             )
                    )
                )
            );

            _randomTile = config.mapGenerationAlgorithm switch
            {
                MapGenerationAlgorithm.UniformRandom => new UniformRandomGenerator(config.seed),
                MapGenerationAlgorithm.Perlin => new PerlinRandomGenerator(config.seed, config.scale, config.offset),
                _ => new UniformRandomGenerator(config.seed)
            };
            _randomResource = new UniformRandomGenerator(config.seed);
            
            _tileSelector = config.tiles.ToWeightedSelector(t => t.weight, WeightedSelectMethod.Alias);
            _resourcesSelectors = resourcesPerTileType.ToDictionary(
                tr => tr.Type,
                tr => tr.Resources.ToWeightedSelector(r => r.weight, WeightedSelectMethod.Alias)
            );
        }

        public TileConfig GenerateTile(int x, int y)
        {
            TileWithWeight tile = _tileSelector.SelectItem(_randomTile.Get(x, y));

            ResourceWithWeight resource = null;
            if (_resourcesSelectors.ContainsKey(tile.type))
            {
                resource = _resourcesSelectors[tile.type].SelectItem(_randomResource.Get(x, y));
            }

            return new TileConfig
            {
                type = tile.type,
                tileResource = resource?.resource ?? TileResourceType.None,
                lootTable = resource?.lootTable,
                nLoots = resource?.nLoots ?? Vector2Int.zero
            };
        }
    }
}
