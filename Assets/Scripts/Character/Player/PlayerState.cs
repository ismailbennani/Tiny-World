using System;

namespace Character.Player
{
    [Serializable]
    public class PlayerState: CharacterState
    {
        public PlayerState(PlayerConfig playerConfig) : base(playerConfig)
        {
        }
    }
}
