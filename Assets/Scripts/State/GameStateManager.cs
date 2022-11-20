using System;
using System.Collections;
using Character.Player;
using MackySoft.Choice;
using Map;
using Map.Tile;
using UnityEngine;
using Utils;

namespace State
{
    [Serializable]
    public class GameStateManager: MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        public static GameState Current => Instance ? Instance.current : null;
        public static bool Ready => Instance && Instance.ready;

        public GameState current;
        public bool ready;

        void OnEnable()
        {
            Instance = this;
            current ??= ScriptableObject.CreateInstance<GameState>();
        }

        void Update()
        {
            if (!Current)
            {
                return;
            }
            
            Current.player?.Update();
        }

        public static IEnumerator CreateNewGame(MapConfig mapConfig, PlayerConfig playerConfig)
        {
            Instance.ready = false;
            
            Debug.Log("Creating new game...");

            if (mapConfig.mapSize.x == 0 || mapConfig.mapSize.y == 0)
            {
                throw new InvalidOperationException("Invalid map size");
            }

            Current.map = new MapState
            {
                config = mapConfig
            };

            int nTiles = mapConfig.mapSize.x * mapConfig.mapSize.y;
            Current.map.tiles = new TileConfig[nTiles];
            Current.map.mapOrigin = Vector3.zero;

            Debug.Log("Generating tiles...");

            if (mapConfig.tiles.Length == 0)
            {
                throw new InvalidOperationException("No tiles provided");
            }
        
            for (int x = 0; x < mapConfig.mapSize.x; x++)
            for (int y = 0; y < mapConfig.mapSize.y; y++)
            {
                TileConfig tileConfig = mapConfig.tiles.ToWeightedSelector(t => t.weight).SelectItemWithUnityRandom().tileConfig;
            
                int index = MyMath.GetIndex(x, y, mapConfig.mapSize);
                Current.map.tiles[index] = tileConfig;
            }
        
            Debug.Log("Done.");

            Debug.Log("Initializing player state...");
            
            Current.player = new PlayerState();
            
            Debug.Log("Done.");
            
            Instance.ready = true;
            Debug.Log("Game state READY");
            
            yield break;
        }
    }
}
