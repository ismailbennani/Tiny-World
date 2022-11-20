using System;
using System.Collections;
using Map.Tile;
using State;
using UnityEngine;
using Utils;

namespace Map
{
    public class MapBuilder : MonoBehaviour
    {
        public float Progress { get; private set; }
        public bool Ready { get; private set; }

        private MapTile[] _tiles;

        public IEnumerator Spawn()
        {
            Ready = false;

            MapState map = GameStateManager.Current.map;
            MapConfig config = map.config;

            if (!config.baseTile)
            {
                throw new InvalidOperationException("Base tile not set");
            }

            _tiles = new MapTile[config.mapSize.x * config.mapSize.y];

            int nTiles = config.mapSize.x * config.mapSize.y;
            Vector2 tileAndGap = config.tileSize + config.gap;

            double maxTimeUsedInThisFrame = Time.fixedDeltaTime * 0.9;
            double yieldAfter = 0;

            for (int x = 0; x < config.mapSize.x; x++)
            for (int y = 0; y < config.mapSize.y; y++)
            {
                if (Time.time > yieldAfter)
                {
                    Progress = (float)(x * config.mapSize.y + y) / nTiles;
                    yield return new WaitForFixedUpdate();
                    yieldAfter = Time.fixedTime + maxTimeUsedInThisFrame;
                }

                Vector3 position = map.mapOrigin + new Vector3(x * tileAndGap.x, 0, y * tileAndGap.y);

                MapTile newTile = Instantiate(config.baseTile, position, Quaternion.identity, transform);

                TileConfig tileConfig = map.GetTileConfig(x, y);
                newTile.SetConfig(tileConfig);

                _tiles[MyMath.GetIndex(x, y, config.mapSize)] = newTile;
            }

            Ready = true;
        }

        public Vector3 GetTileCenterPosition(Vector2Int tilePosition)
        {
            int index = MyMath.GetIndex(tilePosition, GameStateManager.Current.map.config.mapSize);
            return _tiles[index].transform.position;
        }
    }
}
