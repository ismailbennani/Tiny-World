using System;
using Cinemachine;
using UnityEngine;

public class PlayerSpawner: MonoBehaviour
{
    public ThirdPersonController playerPrefab;
    public Vector2 spawnPosition;

    [Header("Camera")]
    public CinemachineVirtualCamera mainCamera;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        Spawn(spawnPosition);
    }

    void Spawn(Vector2 position)
    {
        ThirdPersonController player = Instantiate(playerPrefab, position, Quaternion.identity, transform);
        
        if (!mainCamera)
        {
            throw new InvalidOperationException("Please set camera");
        }
        
        mainCamera.Follow = player.cameraTarget;
        mainCamera.LookAt = player.cameraTarget;
    }
}