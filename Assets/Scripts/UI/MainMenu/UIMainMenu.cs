using UI.Theme;
using UnityEngine;

namespace UI.MainMenu
{
    public class UIMainMenu : UIWindow
    {
        [Header("Main window")]
        public UIButton[] menuItems;

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
