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
            if (_tiles != null)
            {
                foreach (MapTile tile in _tiles)
                {
                    Destroy(tile.gameObject);
                }
            }
            
            Ready = false;

            MapState map = GameStateManager.Current.map;
            MapConfig config = map.config;

            if (!config.baseTile)
            {
                throw new InvalidOperationException("Base tile not set");
            }
            
            int nTiles = (config.mapSize.x + 2) * (config.mapSize.y + 2);
            _tiles = new MapTile[nTiles];

            double maxTimeUsedInThisFrame = Time.fixedDeltaTime * 0.9;
            double yieldAfter = 0;

            for (int index = 0; index < map.tiles.Length; index++)
            {
                if (Time.time > yieldAfter)
                {
                    Progress = (float)index / nTiles;
                    yield return new WaitForFixedUpdate();
                    yieldAfter = Time.fixedTime + maxTimeUsedInThisFrame;
                }

                Vector3 position = map.GetTileCenterPosition(index);
                MapTile newTile = Instantiate(config.baseTile, position, Quaternion.identity, transform);

                TileState tileState = map.tiles[index];
                newTile.SetConfig(tileState);

                _tiles[index] = newTile;
            }

            Ready = true;
        }
    }
}
