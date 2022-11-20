using State;
using UnityEngine;

namespace Character
{
    public class PlayerPersistPosition: MonoBehaviour
    {
        private void FixedUpdate()
        {
            GameState.Current.player.position = transform.position;
        }
    }
}
