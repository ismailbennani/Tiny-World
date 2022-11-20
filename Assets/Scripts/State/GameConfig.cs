using Character.Player;
using Map;
using UnityEngine;

namespace State
{
    [CreateAssetMenu(menuName = "Custom/Game config")]
    public class GameConfig: ScriptableObject
    {
        public MapConfig map;
        public PlayerConfig player;
    }
}
