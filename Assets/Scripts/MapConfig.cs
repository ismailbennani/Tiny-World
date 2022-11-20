using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Map config")]
public class MapConfig: ScriptableObject
{
    public SpawnableTerrain[] terrains;

    [Header("Map config")]
    public Vector2 tileSize;
    public Vector2 gap;
    public Vector2Int desiredMapSize;
}