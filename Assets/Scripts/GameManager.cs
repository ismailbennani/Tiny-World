using System;
using System.Collections;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    public MapConfig mapConfig;
    
    private TerrainSpawner _terrainSpawner;
    private PlayerSpawner _playerSpawner;

    IEnumerator Start()
    {
        _terrainSpawner = GetOrCreate<TerrainSpawner>("Terrain");
        _terrainSpawner.mapConfig = mapConfig;
        
        StartCoroutine(_terrainSpawner.Spawn());

        while (!_terrainSpawner.Ready)
        {
            yield return null;
        }
        
        _playerSpawner = FindObjectOfType<PlayerSpawner>() ?? throw new InvalidOperationException($"Please instantiate a {nameof(PlayerSpawner)}");
        StartCoroutine(_playerSpawner.Spawn());

        while (!_playerSpawner.Ready)
        {
            yield return null;
        }
    }

    private T GetOrCreate<T>(string gameObjectName) where T: Component
    {
        T result = FindObjectOfType<T>();

        if (result == null)
        {
            GameObject obj = new GameObject(gameObjectName, typeof(T));
            result = obj.GetComponent<T>();
        }

        return result;
    }
}