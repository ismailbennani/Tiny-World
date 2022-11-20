using UnityEngine;

namespace Character.Player
{
    [CreateAssetMenu(menuName = "Custom/Player config")]
    public class PlayerConfig: ScriptableObject
    {
        public ThirdPersonController prefab;
        public Vector2Int spawnTile;
    }
}
