using Character.Player;
using Items;
using Map;
using UI.Theme;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Game config")]
public class GameConfig: ScriptableObject
{
    [Header("Map")]
    public MapInitialConfig mapInitialConfig;
    public MapRuntimeConfig mapRuntimeConfig;
    
    [Header("Player")]
    public PlayerConfig player;
    
    [Header("Items")]
    public ItemsRuntimeConfig items;

    [Header("UI")]
    public UITheme theme;
}