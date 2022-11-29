using UI.Theme;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class UIMainMenu : UIWindow
    {
        [Header("Main window")]
        public UIButton[] menuItems;

        protected override void OnFocus()
        {
            EventSystem.current.SetSelectedGameObject(menuItems.Length > 0 ? menuItems[0].gameObject : closeButton.gameObject);
        }

        protected override void SetThemeInternal(UITheme theme)
        {
            if (menuItems != null)
            {
                foreach (UIButton menuItem in menuItems)
                {
                    menuItem.SetTheme(theme);
                }
            }
        }

        protected override void SaveThemeInternal(UITheme theme)
        {
            if (menuItems != null && menuItems.Length > 0)
            {
                UIButton menuItem = menuItems[0];
                menuItem.SaveTheme(theme);
            }
        }
    }
}
