using UnityEngine;

namespace Character.Player
{
    public class PlayerPersistPosition: MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (!GameStateManager.Current)
            {
                return;
            }
            
            GameStateManager.Current.player.position = transform.position;
        }
    }
}
