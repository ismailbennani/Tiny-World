using UI.Theme;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class UIMainMenu : UIWindow
    {
        [Header("Main window")]
        public UIButton[] menuItems;

        protected override void OnOpen()
        {
            base.OnOpen();

            if (menuItems.Length > 0)
            {
                EventSystem.current.SetSelectedGameObject(menuItems[0].gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
            }
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
