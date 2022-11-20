using System;
using System.Collections;
using MackySoft.Choice;
using Map;
using Map.Tile;
using State;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    public MapConfig mapConfig;

    private MapBuilder _mapBuilder;
    private PlayerSpawner _playerSpawner;

    void Start()
    {
        StartCoroutine(CreateNewGame());
    }

    private IEnumerator CreateNewGame()
    {
        Debug.Log("Initializing state...");
        
        GameState state = GameState.Initialize();

        if (mapConfig.mapSize.x == 0 || mapConfig.mapSize.y == 0)
        {
            throw new InvalidOperationException("Invalid map size");
        }

        state.map.config = mapConfig;

        int nTiles = mapConfig.mapSize.x * mapConfig.mapSize.y;
        state.map.tiles = new TileConfig[nTiles];
        
        Debug.Log("Done.");

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
            state.map.tiles[index] = tileConfig;
        }
        
        Debug.Log("Done.");

        Debug.Log("Spawning map...");
        
        _mapBuilder = GetOrCreate<MapBuilder>("Terrain");
        StartCoroutine(_mapBuilder.Spawn());

        while (!_mapBuilder.Ready)
        {
            Debug.Log($"Progress {_mapBuilder.Progress:P2}");
            yield return null;
        }
        
        Debug.Log("Done.");
        
        Debug.Log("Spawning player...");

        _playerSpawner = FindObjectOfType<PlayerSpawner>() ?? throw new InvalidOperationException($"Please instantiate a {nameof(PlayerSpawner)}");
        StartCoroutine(_playerSpawner.Spawn());

        while (!_playerSpawner.Ready)
        {
            yield return null;
        }
        
        Debug.Log("Done.");
        
        Debug.Log("Game READY");
    }

    private T GetOrCreate<T>(string gameObjectName) where T: Component
    {
        T result = FindObjectOfType<T>();

        if (result == null)
        {
            GameObject obj = new(gameObjectName, typeof(T));
            result = obj.GetComponent<T>();
        }

        return result;
    }
}
