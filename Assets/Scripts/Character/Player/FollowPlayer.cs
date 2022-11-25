using UnityEngine;

namespace Character.Player
{
    public class FollowPlayer: MonoBehaviour
    {
        void Update()
        {
            GameState state = GameStateManager.Current;
            if (!state)
            {
                return;
            }

            PlayerState player = state.player;
            if (player == null)
            {
                return;
            }

            transform.position = player.position;
        }
    }
}
