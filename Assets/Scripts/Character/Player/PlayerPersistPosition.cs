using State;
using UnityEngine;

namespace Character.Player
{
    public class PlayerPersistPosition: MonoBehaviour
    {
        private void FixedUpdate()
        {
            GameState.Current.player.position = transform.position;
        }
    }
}
