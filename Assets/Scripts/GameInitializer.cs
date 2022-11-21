using System.Collections;
using Character.Player;
using Map;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private Map.GameMap _gameMap;
    private PlayerSpawner _playerSpawner;

    void OnEnable()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        while (!GameStateManager.Ready)
        {
            yield return null;
        }

        Debug.Log("Spawning map...");
        
        _gameMap = GetOrCreate<GameMap>("Terrain");
        _gameMap.Initialize();

        while (!_gameMap.Ready)
        {
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
