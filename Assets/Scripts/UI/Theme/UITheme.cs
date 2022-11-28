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
        public Sprite button;
        public Sprite buttonHighlighted;
        public Sprite buttonPressed;
        public Sprite buttonDisabled;
        public Sprite cursor;
        
        [Header("Close button")]
        public Sprite closeButton;
        public Sprite closeButtonIcon;

        [Header("Nested")]
        public Sprite nestedPanel;
    }
}
