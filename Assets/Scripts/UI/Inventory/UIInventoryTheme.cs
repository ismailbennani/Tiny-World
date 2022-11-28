using UI.Theme;
using UnityEngine;

namespace UI.Inventory
{
    [CreateAssetMenu(menuName = "Custom/Theme/Inventory")]
    public class UIInventoryTheme: ScriptableObject
    {
        [Header("Main panel")]
        public Sprite panel;
        public Sprite closeButton;
        public Sprite closeButtonIcon;
        public UITextTheme title;

        [Header("Grid")]
        public Sprite gridPanel;
        public Sprite gridItemPanel;
        public Sprite gridItemSelectedPanel;
        public UITextTheme gridItemCount;
    }
}
