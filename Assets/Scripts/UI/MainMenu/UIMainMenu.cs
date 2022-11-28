using Input;
using TMPro;
using UI.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class UIMainMenu : UIWindow
    {
        public static UIMainMenu Instance { get; private set; }

        [Header("Main window")]
        public Image panel;
        public Image closeButton;
        public Image closeButtonIcon;
        public TextMeshProUGUI title;

        public UIButton[] menuItems;

        void OnEnable()
        {
            Instance = this;
        }

        protected override void OnOpen()
        {
            GameInputAdapter gameInputAdapter = GameInputAdapter.Instance;
            if (!gameInputAdapter)
            {
                return;
            }
            
            gameInputAdapter.SwitchToUi();
        }

        protected override void OnClose()
        {
            GameInputAdapter gameInputAdapter = GameInputAdapter.Instance;
            if (!gameInputAdapter)
            {
                return;
            }
            
            gameInputAdapter.SwitchToPlayer();
        }

        protected override void SetThemeInternal(UITheme theme)
        {
            panel.sprite = theme.panel;
            closeButton.sprite = theme.closeButton;
            closeButtonIcon.sprite = theme.closeButtonIcon;
            title.font = theme.title.font;
            title.color = theme.title.color;

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
            defaultTheme.panel = panel.sprite;
            defaultTheme.closeButton = closeButton.sprite;
            defaultTheme.closeButtonIcon = closeButtonIcon.sprite;
            defaultTheme.title.font = title.font;
            defaultTheme.title.color = title.color;

            if (menuItems != null && menuItems.Length > 0)
            {
                UIButton menuItem = menuItems[0];
                menuItem.SaveTheme(theme);
            }
        }
    }
}
