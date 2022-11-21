using System;
using System.Collections;
using Character.Player;
using Map;
using UnityEditor;
using UnityEngine;

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
            currentState.map.runtimeConfig = gameConfig.mapRuntimeConfig;
            
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

        if (manager.gameConfig.mapInitialConfig.chunkSize.x == 0 || manager.gameConfig.mapInitialConfig.chunkSize.y == 0)
        {
            throw new InvalidOperationException("Invalid chunk size");
        }

        currentState.map = new MapState(manager.gameConfig);

        if (manager.gameConfig.mapInitialConfig.initialChunks.x != 0 && manager.gameConfig.mapInitialConfig.initialChunks.y != 0)
        {
            Debug.Log("Generating initial tiles...");

            for (int x = 0; x < manager.gameConfig.mapInitialConfig.initialChunks.x; x++)
            for (int y = 0; y < manager.gameConfig.mapInitialConfig.initialChunks.y; y++)
            {
                currentState.map.PrepareChunk(x, y);
            }
            
            Debug.Log("Done.");
        }

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

    public static void ResetStateWithNewSeed()
    {
        GameStateManager manager = Instance;
        if (!manager)
        {
            manager = FindObjectOfType<GameStateManager>();
        }

        manager.gameConfig.mapInitialConfig.seed = new System.Random().Next();

        ResetState();
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

        if (GUILayout.Button("Reset state (same seed)"))
        {
            GameStateManager.ResetState();
        }

        if (GUILayout.Button("Reset state (new seed)"))
        {
            GameStateManager.ResetStateWithNewSeed();
        }
    }
}

    #endif
