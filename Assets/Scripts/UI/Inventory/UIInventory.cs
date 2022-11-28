using Character.Inventory;
using UI.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class UIInventory : UIWindow
    {
        [Header("Grid")]
        public Image gridPanel;
        public UIInventoryGrid grid;

        private InventoryState _inventoryState;

        protected override void SetThemeInternal(UITheme theme)
        {
            gridPanel.sprite = theme.nestedPanel;

            grid.SetTheme(theme);
        }

        protected override void SaveThemeInternal(UITheme theme)
        {
            defaultTheme.nestedPanel = gridPanel.sprite;

            grid.SaveTheme(theme);
        }

        protected override void OnOpen()
        {
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
