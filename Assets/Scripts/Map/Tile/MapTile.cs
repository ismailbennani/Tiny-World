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

        private void OnEnable()
        {
            if (state != null)
            {
                AddListeners(state);
            }
        }

        void Update()
        {
            if (state != null && GameStateManager.Current)
            {
                TileState newState = GameStateManager.Current.map.GetTile(state.position);
                if (newState != state)
                {
                    SetConfig(newState);
                }
            }
        }
        
        public void SetConfig(TileState tileConfig)
        {
            AddListeners(tileConfig);
            
            state = tileConfig;

            if (state == null)
            {
                return;
            }

            SpawnPlatform();
            SpawnResource();
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
            
            if (_resource)
            {
                DestroyGameObject(_resource);
            }
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
            if (clip)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position, volume);
            }
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
            if (_resource)
            {
                DestroyGameObject(_resource);
            }

            if (!state.HasResource)
            {
                return;
            }

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

        private static void DestroyGameObject(Component component)
        {
            if (Application.isPlaying)
            {
                Destroy(component.gameObject);
            }
            else
            {
                DestroyImmediate(component.gameObject);
            }
        }

        private void AddListeners(TileState newState)
        {
            if (state != null)
            {
                state.onConsume.RemoveListener(OnConsume);
                state.onDepleted.RemoveListener(OnDepleted);
            }
            
            newState.onConsume.AddListener(OnConsume);
            newState.onDepleted.AddListener(OnDepleted);
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
