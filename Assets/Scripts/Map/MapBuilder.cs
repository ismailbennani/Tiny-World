using System;
using System.Collections;
using Map.Tile;
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

            GameState gameState = GameState.Current;

            if (!gameState.mapConfig.baseTile)
            {
                throw new InvalidOperationException("Base tile not set");
            }
            
            _tiles = new MapTile[gameState.mapConfig.mapSize.x * gameState.mapConfig.mapSize.y];

            int nTiles = gameState.mapConfig.mapSize.x * gameState.mapConfig.mapSize.y;
            Vector2 tileAndGap = gameState.mapConfig.tileSize + gameState.mapConfig.gap;
            Vector2 worldSize = tileAndGap * gameState.mapConfig.mapSize - gameState.mapConfig.gap;
            Vector2 worldHalfSize = worldSize / 2;


            double maxTimeUsedInThisFrame = Time.fixedDeltaTime * 0.9;
            double yieldAfter = 0;

            for (int x = 0; x < gameState.mapConfig.mapSize.x; x++)
            for (int y = 0; y < gameState.mapConfig.mapSize.y; y++)
            {
                if (Time.time > yieldAfter)
                {
                    Progress = (float)(x * gameState.mapConfig.mapSize.y + y) / nTiles;
                    yield return new WaitForFixedUpdate();
                    yieldAfter = Time.fixedTime + maxTimeUsedInThisFrame;
                }

                Vector3 position = new(x * tileAndGap.x - worldHalfSize.x, 0, y * tileAndGap.y - worldHalfSize.y);

                MapTile newTile = Instantiate(gameState.mapConfig.baseTile, position, Quaternion.identity, transform);

                TileConfig tileConfig = gameState.GetTileAt(x, y);
                newTile.SetConfig(tileConfig);

                _tiles[MyMath.GetIndex(x, y, gameState.mapConfig.mapSize)] = newTile;
            }

            Ready = true;
        }
    }
}
