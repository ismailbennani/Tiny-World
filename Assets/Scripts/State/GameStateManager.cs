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
        public static GameState Current => Instance ? Instance.currentState : null;
        public static bool Ready => Instance && Instance.ready;

        public GameConfig gameConfig;
        public GameState currentState;
        public bool ready;

        void OnEnable()
        {
            Instance = this;
            ready = false;

            if (!currentState || !currentState.IsValid)
            {
                if (!currentState)
                {
                    currentState = ScriptableObject.CreateInstance<GameState>(); 
                }
                
                StartCoroutine(CreateNewGame(gameConfig));
            }
            else
            {
                ready = true;
            }
        }

        void Update()
        {
            if (!Current)
            {
                return;
            }
            
            Current.player?.Update();
        }

        public static IEnumerator CreateNewGame(GameConfig gameConfig)
        {
            GameState currentState = Instance.currentState;
            
            Instance.ready = false;
            
            Debug.Log("Creating new game...");

            if (gameConfig.map.mapSize.x == 0 || gameConfig.map.mapSize.y == 0)
            {
                throw new InvalidOperationException("Invalid map size");
            }

            int nTiles = gameConfig.map.mapSize.x * gameConfig.map.mapSize.y;
            
            currentState.map = new MapState
            {
                config = gameConfig.map,
                tiles = new TileConfig[nTiles],
                mapOrigin = Vector3.zero
            };

            Debug.Log("Generating tiles...");

            if (gameConfig.map.tiles.Length == 0)
            {
                throw new InvalidOperationException("No tiles provided");
            }
        
            for (int x = 0; x < gameConfig.map.mapSize.x; x++)
            for (int y = 0; y < gameConfig.map.mapSize.y; y++)
            {
                TileConfig tileConfig = gameConfig.map.tiles.ToWeightedSelector(t => t.weight).SelectItemWithUnityRandom().tileConfig;
            
                int index = MyMath.GetIndex(x, y, gameConfig.map.mapSize);
                currentState.map.tiles[index] = tileConfig;
            }
        
            Debug.Log("Done.");

            Debug.Log("Initializing player state...");
            
            currentState.player = new PlayerState
            {
                config = gameConfig.player,
                position = currentState.map.GetTileCenterPosition(gameConfig.player.spawnTile)
            };

            Debug.Log("Done.");
            
            Instance.ready = true;
            Debug.Log("Game state READY");
            
            yield break;
        }
    }
}
