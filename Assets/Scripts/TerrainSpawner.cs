using System;
using System.Collections;
using MackySoft.Choice;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public MapConfig mapConfig;

    public bool Ready { get; private set; }

    private Transform[,] _tiles;

    public IEnumerator Spawn()
    {
        if (mapConfig.desiredMapSize.x == 0 || mapConfig.desiredMapSize.y == 0)
        {
            throw new InvalidOperationException($"Invalid map size {mapConfig.desiredMapSize}");
        }

        if (mapConfig.terrains.Length == 0)
        {
            throw new InvalidOperationException("No mapConfig.terrains provided");
        }

        Ready = false;

        _tiles = new Transform[mapConfig.desiredMapSize.x, mapConfig.desiredMapSize.y];

        Vector2 tileAndGap = mapConfig.tileSize + mapConfig.gap;
        Vector2 worldSize = tileAndGap * mapConfig.desiredMapSize - mapConfig.gap;
        Vector2 worldHalfSize = worldSize / 2;

        double maxTimeUsedInThisFrame = Time.fixedDeltaTime * 0.9;
        double yieldAfter = 0;

        for (int x = 0; x < mapConfig.desiredMapSize.x; x++)
        for (int y = 0; y < mapConfig.desiredMapSize.y; y++)
        {
            if (Time.time > yieldAfter)
            {
                yield return new WaitForFixedUpdate();
                yieldAfter = Time.fixedTime + maxTimeUsedInThisFrame;
            }
            
            Transform toSpawn = ChooseTerrain(x, y);
            Vector3 position = new(x * tileAndGap.x - worldHalfSize.x, 0, y * tileAndGap.y - worldHalfSize.y);

            Transform newTile = Instantiate(toSpawn, position, Quaternion.identity, transform);

            _tiles[x, y] = newTile;
        }

        Ready = true;
    }

    private Transform ChooseTerrain(int _, int __)
    {
        return mapConfig.terrains.ToWeightedSelector(t => t.weight).SelectItemWithUnityRandom().prefab;
    }
}

[Serializable]
public class SpawnableTerrain
{
    public Transform prefab;
    public float weight = 1;
}
