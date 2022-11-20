using UnityEngine;

namespace Map.Tile
{
    [CreateAssetMenu(menuName = "Custom/Tile config")]
    public class TileConfig: ScriptableObject
    {
        public TileType type;
    }
}