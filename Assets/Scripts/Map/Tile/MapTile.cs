using System;
using System.Collections.Generic;
using System.Linq;
using Resource;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Map.Tile
{
    public class MapTile : MonoBehaviour
    {
        [Header("Platforms")]
        public List<MapTilePlatformParams> platformsParams;
        
        [Header("Resources")]
        public List<MapTileResourceParams> resourcesParams;

        [Space(10)]
        public TileState state;

        private MapTilePlatform _platform;
        private MapTileResource _resource;

        void Update()
        {
            if (!state.HasResource && _resource)
            {
                DestroyResource();
            }
        }

        public void SetConfig(TileState tileConfig)
        {
            state = tileConfig;

            if (state == null)
            {
                return;
            }

            SpawnPlatform();
            SpawnResource();
            
            state.onConsume.AddListener(OnConsume);
            state.onDepleted.AddListener(OnDepleted);
        }

        private void OnConsume(int quantity)
        {
            PlayConsumeClip();

            if (_resource)
            {
                _resource.OnGather();
            }
        }

        private void OnDepleted()
        {
            PlayDepletedClip();
        }
        
        #region Audio
        
        public void PlayConsumeClip()
        {
            MapTileResourceParams resourceParams = resourcesParams.FirstOrDefault(p => p.resource == state.config.resource);
            if (resourceParams == null)
            {
                return;
            }
            
            PlayClip(resourceParams.consumeAudioClips, resourceParams.consumeAudioVolume);
        }
        
        public void PlayDepletedClip()
        {
            MapTileResourceParams resourceParams = resourcesParams.FirstOrDefault(p => p.resource == state.config.resource);
            if (resourceParams == null)
            {
                return;
            }
            
            PlayClip(resourceParams.depletedAudioClips, resourceParams.depletedAudioVolume);
        }

        private void PlayClip(IReadOnlyList<AudioClip> clips, float volume)
        {
            AudioClip clip = clips.ChooseRandomly();
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        }
        
        #endregion

        #region Setup
        
        private void SpawnPlatform()
        {
            if (_platform)
            {
                DestroyGameObject(_platform);
            }

            MapTilePlatform[] prefabs = platformsParams.SingleOrDefault(m => m.type == state.config.type)?.prefabs;
            if (prefabs == null || prefabs.Length <= 0)
            {
                return;
            }

            MapTilePlatform prefab = prefabs[state.generationConfig.resourceVariant % prefabs.Length];
            if (prefab)
            {
                _platform = Instantiate(prefab, transform);
                _platform.SetTile(state);
            }
        }

        private void SpawnResource()
        {
            DestroyResource();

            MapTileResource[] prefabs = resourcesParams.SingleOrDefault(m => m.resource == state.config.resource)?.prefabs;
            if (prefabs == null || prefabs.Length <= 0)
            {
                return;
            }

            MapTileResource prefab = prefabs[state.generationConfig.resourceVariant % prefabs.Length];
            if (prefab)
            {
                _resource = Instantiate(prefab, transform);
                _resource.SetTile(state);
            }
        }

        private void DestroyResource()
        {
            if (_resource)
            {
                DestroyGameObject(_resource);
            }
        }

        private static void DestroyGameObject(Component component)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(component.gameObject);
            }
            else
            {
                Destroy(component.gameObject);
            }
        }
        
        #endregion
    }
    
    [Serializable]
    public class MapTilePlatformParams
    {
        public TileType type;
        
        [Header("Prefabs")]
        [Tooltip("Available variants for this platform, see TileState.platformVariant")]
        public MapTilePlatform[] prefabs;
    }

    [Serializable]
    public class MapTileResourceParams
    {
        public ResourceType resource;

        [Header("Prefabs")]
        [Tooltip("Available variants for this resource, see TileState.resourceVariant")]
        public MapTileResource[] prefabs;

        [Header("Audio")]
        [Tooltip("One of these will be chosen randomly and played on each resource consumption")]
        public AudioClip[] consumeAudioClips;
        public float consumeAudioVolume;

        [Tooltip("One of these will be chosen randomly and played when resource is depleted")]
        public AudioClip[] depletedAudioClips;
        public float depletedAudioVolume;
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(MapTile))]
    public class MapTileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Set config"))
            {
                MapTile mapTile = target as MapTile;

                mapTile.SetConfig(mapTile.state);
            }
        }
    }

    #endif
}
