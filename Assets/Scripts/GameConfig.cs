using Character;
using Items;
using Map;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Game config")]
public class GameConfig: ScriptableObject
{
    [Header("Map")]
    public MapInitialConfig mapInitialConfig;
    public MapRuntimeConfig mapRuntimeConfig;
    
    [Header("player")]
    public CharacterConfig character;
    
    [Header("Items")]
    public ItemsRuntimeConfig items;
}