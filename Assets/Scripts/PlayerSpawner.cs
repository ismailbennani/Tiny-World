using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public ThirdPersonController playerPrefab;
    public Vector2 spawnPosition;

    [Header("Camera")]
    public CinemachineVirtualCamera mainCamera;

    public bool Ready { get; private set; }
    
    public IEnumerator Spawn()
    {
        IEnumerator spawn = Spawn(spawnPosition);
        while (spawn.MoveNext())
        {
            yield return spawn.Current;
        }
    }

    public IEnumerator Spawn(Vector2 position)
    {
        Ready = false;
        
        ThirdPersonController player = Instantiate(playerPrefab, position, Quaternion.identity, transform);

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
