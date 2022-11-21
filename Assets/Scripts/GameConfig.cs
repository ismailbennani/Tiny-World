using Character.Player;
using Map;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Game config")]
public class GameConfig: ScriptableObject
{
    public MapInitialConfig mapInitialConfig;
    public MapRuntimeConfig mapRuntimeConfig;
    public PlayerConfig player;
}