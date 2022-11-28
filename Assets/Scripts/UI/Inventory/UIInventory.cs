using Character.Inventory;
using TMPro;
using UI.MainMenu;
using UI.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class UIInventory : UIWindow
    {
        public static UIInventory Instance { get; private set; }

        [Header("Main window")]
        public Image panel;
        public Image closeButton;
        public Image closeButtonIcon;
        public TextMeshProUGUI title;

        [Header("Grid")]
        public Image gridPanel;
        public UIInventoryGrid grid;

        private InventoryState _inventoryState;

        private void OnEnable()
        {
            Instance = this;
        }

        protected override void SetThemeInternal(UITheme theme)
        {
            panel.sprite = theme.panel;
            closeButton.sprite = theme.closeButton;
            closeButtonIcon.sprite = theme.closeButtonIcon;
            title.font = theme.title.font;
            title.color = theme.title.color;

            gridPanel.sprite = theme.nestedPanel;
            grid.GridItemDefaultPanel = theme.button;
            grid.GridItemSelectedPanel = theme.buttonPressed;
            grid.GridItemCountFont = theme.text.font;
            grid.GridItemCountColor = theme.text.color;
        }

        protected override void SaveThemeInternal(UITheme theme)
        {
            defaultTheme.panel = panel.sprite;
            defaultTheme.closeButton = closeButton.sprite;
            defaultTheme.closeButtonIcon = closeButtonIcon.sprite;
            defaultTheme.title.font = title.font;
            defaultTheme.title.color = title.color;

            defaultTheme.nestedPanel = gridPanel.sprite;
            defaultTheme.button = grid.GridItemDefaultPanel;
            defaultTheme.buttonPressed = grid.GridItemSelectedPanel;
            defaultTheme.text.font = grid.GridItemCountFont;
            defaultTheme.text.color = grid.GridItemCountColor;
        }

        protected override void OnOpen()
        {
            UIMainMenu uiMainMenu = UIMainMenu.Instance;
            if (uiMainMenu)
            {
                uiMainMenu.Close();
            }

            UpdateInventory();

            _inventoryState?.onChange.AddListener(OnInventoryChange);
        }

        protected override void OnClose()
        {
            root.SetActive(false);

            _inventoryState?.onChange.RemoveListener(OnInventoryChange);
        }

        private void UpdateInventory()
        {
            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            _inventoryState = gameState.player?.inventoryState;
            if (_inventoryState == null)
            {
                return;
            }

            grid.UpdateItems(_inventoryState);
        }

        private void OnInventoryChange(InventoryLine _)
        {
            UpdateInventory();
        }
    }
}
