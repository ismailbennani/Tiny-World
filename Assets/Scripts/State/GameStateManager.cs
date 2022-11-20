using System;
using System.Collections;
using Character.Player;
using MackySoft.Choice;
using Map.Tile;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace State
{
    [Serializable]
    public class GameStateManager : MonoBehaviour
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

                StartCoroutine(CreateNewGame(this));
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

        public static IEnumerator CreateNewGame(GameStateManager manager)
        {
            GameState currentState = manager.currentState;

            manager.ready = false;

            Debug.Log("Creating new game...");

            if (manager.gameConfig.map.mapSize.x == 0 || manager.gameConfig.map.mapSize.y == 0)
            {
                throw new InvalidOperationException("Invalid map size");
            }

            int nTiles = (manager.gameConfig.map.mapSize.x + 2) * (manager.gameConfig.map.mapSize.y + 2);

            currentState.map = new MapState
            {
                config = manager.gameConfig.map,
                tiles = new TileState[nTiles],
                mapOrigin = Vector3.zero
            };

            Debug.Log("Generating tiles...");

            if (manager.gameConfig.map.tiles.Length == 0)
            {
                throw new InvalidOperationException("No tiles provided");
            }

            Vector2Int sizeWithBorders = manager.gameConfig.map.mapSize + 2 * Vector2Int.one;

            for (int x = 0; x < sizeWithBorders.x; x++)
            for (int y = 0; y < sizeWithBorders.y; y++)
            {
                TileConfig tileConfig = x == 0 || y == 0 || x == sizeWithBorders.x - 1 || y == sizeWithBorders.y - 1
                    ? TileConfig.Empty
                    : manager.gameConfig.map.tiles.ToWeightedSelector(t => t.weight).SelectItemWithUnityRandom().tileConfig;

                int index = MyMath.GetIndex(x, y, sizeWithBorders);
                currentState.map.tiles[index] = new TileState
                {
                    config = tileConfig,
                    position = new Vector2Int(x, y),
                    rotation = Random.Range(0, 4),
                };
            }

            Debug.Log("Done.");

            Debug.Log("Initializing player state...");

            currentState.player = new PlayerState
            {
                config = manager.gameConfig.player,
                position = currentState.map.GetTileCenterPosition(manager.gameConfig.player.spawnTile)
            };

            Debug.Log("Done.");

            manager.ready = true;
            Debug.Log("Game state READY");

            yield break;
        }

        public static void ResetPlayerPosition()
        {
            GameState state = Current;
            PlayerController controller = FindObjectOfType<PlayerController>();

            controller.transform.position = state.map.GetTileCenterPosition(state.player.config.spawnTile);
        }

        public static void ResetState()
        {
            GameStateManager manager = Instance;
            if (!manager)
            {
                manager = FindObjectOfType<GameStateManager>();
            }

            IEnumerator coroutine = CreateNewGame(manager);
            while (coroutine.MoveNext())
            {
                //
            }
        }
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(GameStateManager))]
    public class GameStateManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Reset player position"))
            {
                GameStateManager.ResetPlayerPosition();
            }

            if (GUILayout.Button("Reset state"))
            {
                GameStateManager.ResetState();
            }
        }
    }

    #endif
}
