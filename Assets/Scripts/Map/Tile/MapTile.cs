using System;
using System.Collections.Generic;
using System.Linq;
using Items;
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
        public GameState gameState;

        public bool hasPosition;
        public Vector2Int tilePosition;

        public TileState State => hasPosition && gameState ? gameState.map.GetTile(tilePosition) : null;

        private MapTilePlatform _platform;
        private MapTileResource _tileResource;


        private TileType _currentPlatform;
        private uint _currentPlatformVariant;
        private uint _currentPlatformRotation;

        private TileResourceType _currentTileResource;
        private uint _currentResourceVariant;
        private uint _currentResourceRotation;

        private void OnEnable()
        {
            TileState tileState = State;
            if (tileState != null)
            {
                AddListeners(tileState);
            }
        }

        public void UpdateState(TileState newState)
        {
            if (newState == null)
            {
                hasPosition = false;
                return;
            }

            AddListeners(newState);
            SpawnPlatformIfNecessary(newState);
            SpawnResourceIfNecessary(newState);

            tilePosition = newState.position;
            hasPosition = true;
        }

        private void OnLoot(Item[] items)
        {
            PlayLootClip();

            if (items == null || items.Length == 0 || !hasPosition)
            {
                return;
            }

            foreach (Item item in items)
            {
                GameMap.Instance.SpawnItem(item, transform.position);
            }

            if (_tileResource)
            {
                _tileResource.OnLoot();
            }
        }

        private void OnDepleted()
        {
            PlayDepletedClip();
            DestroyGameObject(_tileResource);
        }

        #region Audio

        public void PlayLootClip()
        {
            TileState tileState = State;
            if (tileState == null)
            {
                return;
            }

            MapTileResourceParams resourceParams = resourcesParams.FirstOrDefault(p => p.tileResource == tileState.config.tileResource);
            if (resourceParams == null)
            {
                return;
            }

            PlayClip(resourceParams.lootAudioClips, resourceParams.lootAudioVolume);
        }

        public void PlayDepletedClip()
        {
            TileState tileState = State;
            if (tileState == null)
            {
                return;
            }

            MapTileResourceParams resourceParams = resourcesParams.FirstOrDefault(p => p.tileResource == tileState.config.tileResource);
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

        private void SpawnPlatformIfNecessary(TileState newState)
        {
            if (newState == null)
            {
                DestroyGameObject(_platform);
                return;
            }

            if (newState.config.type == _currentPlatform
                && newState.generationConfig.platformVariant == _currentPlatformVariant
                && newState.generationConfig.platformRotation == _currentPlatformRotation)
            {
                return;
            }

            _currentPlatform = newState.config.type;
            _currentPlatformVariant = newState.generationConfig.platformVariant;

            DestroyGameObject(_platform);

            MapTilePlatform[] prefabs = platformsParams.SingleOrDefault(m => m.type == newState.config.type)?.prefabs;
            if (prefabs == null || prefabs.Length <= 0)
            {
                return;
            }

            MapTilePlatform prefab = prefabs[newState.generationConfig.platformVariant % prefabs.Length];
            if (prefab)
            {
                _platform = Instantiate(prefab, transform);

                int rotation = (int)(newState.generationConfig.platformRotation % 4);
                _platform.transform.rotation = Quaternion.Euler(0, 90 * rotation, 0);

                _platform.SetTile(newState);
            }
        }

        private void SpawnResourceIfNecessary(TileState newState)
        {
            if (newState == null)
            {
                DestroyGameObject(_tileResource);
                return;
            }

            if (newState.config.tileResource == _currentTileResource
                && newState.generationConfig.resourceVariant == _currentResourceVariant
                && newState.generationConfig.resourceRotation == _currentResourceRotation)
            {
                return;
            }

            _currentTileResource = newState.config.tileResource;
            _currentResourceVariant = newState.generationConfig.resourceVariant;
            _currentPlatformRotation = newState.generationConfig.platformRotation;

            DestroyGameObject(_tileResource);

            if (!newState.IsLootable)
            {
                return;
            }

            MapTileResource[] prefabs = resourcesParams
                .SingleOrDefault(m => m.tileResource == newState.config.tileResource && m.tile.Check(newState.config.type))?.prefabs;
            if (prefabs == null || prefabs.Length <= 0)
            {
                return;
            }

            MapTileResource prefab = prefabs[newState.generationConfig.resourceVariant % prefabs.Length];
            if (prefab)
            {
                _tileResource = Instantiate(prefab, transform);

                int rotation = (int)(newState.generationConfig.resourceRotation % 4);
                _tileResource.transform.rotation = Quaternion.Euler(0, 90 * rotation, 0);

                _tileResource.SetTile(newState);
            }
        }

        private static void DestroyGameObject(Component component)
        {
            if (!component)
            {
                return;
            }

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
            if (State != null)
            {
                State.onLoot.RemoveListener(OnLoot);
                State.onDepleted.RemoveListener(OnDepleted);
            }

            newState.onLoot.AddListener(OnLoot);
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
        public TileResourceType tileResource;
        public TileTypeMask tile;

        [Header("Prefabs")]
        [Tooltip("Available variants for this resource, see TileState.resourceVariant")]
        public MapTileResource[] prefabs;

        [Header("Audio")]
        [Tooltip("One of these will be chosen randomly and played on each resource consumption")]
        public AudioClip[] lootAudioClips;
        public float lootAudioVolume;

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

                mapTile.UpdateState(mapTile.State);
            }
        }
    }

    #endif
}
