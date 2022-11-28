using UnityEngine;

namespace UI.Theme
{
    [CreateAssetMenu(menuName = "Custom/Theme/Inventory")]
    public class UITheme: ScriptableObject
    {
        [Header("Main panel")]
        public Sprite panel;
        public UITextTheme title;
        public UITextTheme text;

        [Header("Interaction")]
        public UIButtonTheme button;
        public Sprite cursor;
        public UIButtonTheme closeButton;
        public Sprite closeButtonIcon;

        [Header("Nested")]
        public Sprite nestedPanel;
    }
}
