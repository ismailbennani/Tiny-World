using Character;
using Items;
using Map;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Game state")]
public class GameState: ScriptableObject
{
    public MapState map;
    public CharacterState character;

    [Header("Other configs")]
    public ItemsRuntimeConfig itemsConfig;

    public bool IsValid => MapIsValid && PlayerIsValid;

    private bool MapIsValid => map?.IsValid ?? false;
    private bool PlayerIsValid => character?.config != null && character.config.prefab;
}