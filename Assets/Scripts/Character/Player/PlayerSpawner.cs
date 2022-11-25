using System.Collections;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        public bool Ready { get; private set; }
        
        private ThirdPersonController player;

        public IEnumerator Spawn()
        {
            Ready = false;

            if (!player)
            {
                PlayerState playerState = GameStateManager.Current.player;
                PlayerConfig config = playerState.config;

                player = Instantiate(config.prefab, playerState.position, Quaternion.identity, transform);
            }
        
            Ready = true;

            yield break;
        }
    }
}
