using Character.Player;
using Map;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Game state")]
public class GameState: ScriptableObject
{
    public MapState map;
    public PlayerState player;

    public bool IsValid => MapIsValid && PlayerIsValid;

    private bool MapIsValid => map?.IsValid ?? false;
    private bool PlayerIsValid => player?.config != null && player.config.prefab;
}