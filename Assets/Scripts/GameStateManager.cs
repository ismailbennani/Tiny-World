using System;
using System.Collections;
using System.Linq;
using Character.Player;
using Map;
using Map.Tile;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public static GameConfig Config => Instance ? Instance.gameConfig : null;
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

        currentState.character = new PlayerState(manager.gameConfig.player)
        {
            position = currentState.map.GetTileCenterPosition(manager.gameConfig.player.spawnTile)
        };

        Debug.Log("Done.");
        
        Debug.Log("Copying other configs...");

        currentState.itemsConfig = manager.gameConfig.items;
        
        Debug.Log("Done.");

        manager.ready = true;
        Debug.Log("Game state READY");

        yield break;
    }

    public static void ResetAllResources()
    {
        GameStateManager manager = Instance;
        if (!manager)
        {
            manager = FindObjectOfType<GameStateManager>();
        }

        foreach (TileState tile in manager.currentState.map.chunks.SelectMany(c => c.tiles))
        {
            tile.remainingLootAttempts = 10;
        }
    }

    public static void ResetPlayerPosition()
    {
        GameStateManager manager = Instance;
        if (!manager)
        {
            manager = FindObjectOfType<GameStateManager>();
        }
        
        manager.currentState.character.position = manager.currentState.map.GetTileCenterPosition(manager.currentState.character.config.spawnTile);

        PlayerController controller = FindObjectOfType<PlayerController>();
        if (controller)
        {
            controller.transform.position = manager.currentState.map.GetTileCenterPosition(manager.currentState.character.config.spawnTile);
        }
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

        if (GUILayout.Button("Reset all resources"))
        {
            GameStateManager.ResetAllResources();
        }

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
