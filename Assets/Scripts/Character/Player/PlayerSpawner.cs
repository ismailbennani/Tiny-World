using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Camera")]
        public CinemachineVirtualCamera mainCamera;

        public bool Ready { get; private set; }
        
        private ThirdPersonController player;

        public IEnumerator Spawn()
        {
            mainCamera = FindObjectOfType<CinemachineVirtualCamera>() ?? throw new InvalidOperationException("No camera found");
            
            Ready = false;

            if (!player)
            {
                PlayerState playerState = GameStateManager.Current.player;
                PlayerConfig config = playerState.config;

                player = Instantiate(config.prefab, playerState.position, Quaternion.identity, transform);
            }

            if (!mainCamera)
            {
                throw new InvalidOperationException("Please set camera");
            }

            mainCamera.Follow = player.cameraTarget;
            mainCamera.LookAt = player.cameraTarget;
        
            Ready = true;

            yield break;
        }
    }
}
