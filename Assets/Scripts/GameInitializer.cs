using System;
using System.Collections;
using Character.Player;
using Map;
using State;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private MapBuilder _mapBuilder;
    private PlayerSpawner _playerSpawner;

    void OnEnable()
    {
        if (GameStateManager.Current == null)
        {
            throw new InvalidOperationException("Could not find game state");
        }

        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        while (!GameStateManager.Ready)
        {
            yield return null;
        }

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
        
        _playerSpawner = GetOrCreate<PlayerSpawner>("Player");
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
            obj.transform.SetParent(transform);
            result = obj.GetComponent<T>();
        }

        return result;
    }
}
