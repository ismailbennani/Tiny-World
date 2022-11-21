using Character.Player;
using Map;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Game config")]
public class GameConfig: ScriptableObject
{
    public MapConfig map;
    public PlayerConfig player;
}