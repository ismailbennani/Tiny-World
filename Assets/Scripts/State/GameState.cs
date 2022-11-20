using UnityEngine;

namespace State
{
    [CreateAssetMenu(menuName = "Custom/Game state")]
    public class GameState: ScriptableObject
    {
        public MapState map;
        public PlayerState player;
    }
}
