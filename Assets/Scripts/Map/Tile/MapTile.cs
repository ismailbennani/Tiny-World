using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Map.Tile
{
    public class MapTile : MonoBehaviour
    {
        [Header("Config")]
        public List<PrefabForTileType> platformPrefabs;
        public List<PrefabForResourceType> resourcePrefabs;

        [Space(10)]
        public TileConfig config;

        private MapTilePlatform _platform;
        private MapTileResource _resource;

        public void SetConfig(TileConfig tileConfig)
        {
            config = tileConfig;

            SpawnPlatform();
            SpawnResource();
        }

        private void SpawnPlatform()
        {
            if (_platform)
            {
                DestroyGameObject(_platform);
            }

            MapTilePlatform prefab = platformPrefabs.SingleOrDefault(m => m.type == config.type)?.prefab;
            if (prefab)
            {
                _platform = Instantiate(prefab, transform);
            }
        }

        private void SpawnResource()
        {
            if (_resource)
            {
                DestroyGameObject(_resource);
            }

            MapTileResource prefab = resourcePrefabs.SingleOrDefault(m => m.type == config.resource)?.prefab;
            if (prefab)
            {
                _resource = Instantiate(prefab, transform);
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

                mapTile.SetConfig(mapTile.config);
            }
        }
    }

    #endif
}
