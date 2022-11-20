using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Map.Tile
{
    public class MapTile : MonoBehaviour
    {
        [Header("Config")]
        public List<PlatformForTileType> prefabs;

        [Space(10)]
        public TileConfig config;

        public MapTilePlatform _tilePrefab;

        public void SetConfig(TileConfig tileConfig)
        {
            config = tileConfig;

            SpawnCorrectPrefab();
        }

        private void SpawnCorrectPrefab()
        {
            if (_tilePrefab)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(_tilePrefab.gameObject);
                }
                else
                {
                    Destroy(_tilePrefab.gameObject);
                }
            }

            MapTilePlatform prefab = prefabs.SingleOrDefault(m => m.type == config.type)?.prefab;
            _tilePrefab = Instantiate(prefab, transform);
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
