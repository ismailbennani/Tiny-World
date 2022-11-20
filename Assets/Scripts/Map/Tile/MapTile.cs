using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Map.Tile
{
    public class MapTile : MonoBehaviour
    {
        public MeshRenderer tileRenderer;

        [Header("Config")]
        public List<MaterialForTileType> materials;
        
        [Space(10)]
        public TileConfig config;

        public void SetConfig(TileConfig tileConfig)
        {
            config = tileConfig;
            
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            Material material = materials.SingleOrDefault(m => m.type == config.type)?.material;
            tileRenderer.material = material;
        }
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(MapTile))]
    // ^ This is the script we are making a custom editor for.
    public class YourScriptEditor : Editor
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
