using System;
using System.Collections;
using Cinemachine;
using State;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Camera")]
        public CinemachineVirtualCamera mainCamera;

        public bool Ready { get; private set; }

        public IEnumerator Spawn()
        {
            mainCamera = FindObjectOfType<CinemachineVirtualCamera>() ?? throw new InvalidOperationException("No camera found");
            
            Ready = false;
            
            PlayerState player = GameState.Current.player;
            PlayerConfig config = player.config;
        
            ThirdPersonController controller = Instantiate(config.prefab, player.position, Quaternion.identity, transform);

            if (!mainCamera)
            {
                throw new InvalidOperationException("Please set camera");
            }

            mainCamera.Follow = controller.cameraTarget;
            mainCamera.LookAt = controller.cameraTarget;
        
            Ready = true;

            yield break;
        }
    }
}
