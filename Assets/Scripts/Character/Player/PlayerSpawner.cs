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
                CharacterState characterState = GameStateManager.Current.player;
                CharacterConfig config = characterState.config;

                player = Instantiate(config.prefab, characterState.position, Quaternion.identity, transform);
            }

            if (!mainCamera)
            {
                throw new InvalidOperationException("Please set camera");
            }

            mainCamera.Follow = player.cameraTarget;
            mainCamera.LookAt = player.cameraTarget;

            CharacterItemsDetector itemDetector = player.GetComponentInChildren<CharacterItemsDetector>();
            itemDetector.highlightClosestItem = true;
        
            Ready = true;

            yield break;
        }
    }
}
