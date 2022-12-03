using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Animation;

namespace Map.Tile.Animation
{
    public class MapTileResourceShakeAnimation: ShakeAnimation, IMapTileResourceAnimation
    {
        public float duration = 0.1f;
        
        public void OnLoot()
        {
            StartCoroutine(Shake(duration));
        }
    }
    
    #if UNITY_EDITOR

    [CustomEditor(typeof(MapTileResourceShakeAnimation))]
    public class MapTileResourceShakeAnimationEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new();

            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            return container;
        }
    }
    
    #endif
}
