using System.Collections.Generic;
using Character.Inventory;
using UI.Theme;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class UIInventory : UIWindow
    {
        public UIInventoryGridItem gridItemPrefab;
        public Transform gridRoot;

        [Header("Grid")]
        public Image gridPanel;

        private InventoryState _inventoryState;
        private List<UIInventoryGridItem> _gridItems = new();

        void OnEnable()
        {
            gridItemPrefab.gameObject.SetActive(false);
        }
        
        protected override void SetThemeInternal(UITheme theme)
        {
            gridPanel.sprite = theme.nestedPanel;

            foreach (UIInventoryGridItem item in _gridItems)
            {
                item.SetTheme(theme);
            }
        }

        protected override void SaveThemeInternal(UITheme theme)
        {
            defaultTheme.nestedPanel = gridPanel.sprite;

            if (_gridItems.Count > 0)
            {
                _gridItems[0].SaveTheme(theme);
            }
        }

        protected override void OnOpen()
        {
            UpdateInventory();

            _inventoryState?.onChange.AddListener(OnInventoryChange);
        }

        protected override void OnFocus()
        {
            EventSystem.current.SetSelectedGameObject(_gridItems.Count > 0 ? _gridItems[0].gameObject : closeButton.gameObject);
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

            if (_gridItems != null)
            {
                foreach (UIInventoryGridItem gridItem in _gridItems)
                {
                    Destroy(gridItem.gameObject);
                }
            }

            _gridItems = new List<UIInventoryGridItem>();

            foreach (InventoryLine line in _inventoryState.lines)
            {
                UIInventoryGridItem newGridItem = Instantiate(gridItemPrefab, gridRoot);
                newGridItem.SetItem(line.item, line.count);

                newGridItem.gameObject.SetActive(true);
                
                _gridItems.Add(newGridItem);
            }
        }

        private void OnInventoryChange(InventoryLine _)
        {
            UpdateInventory();
        }
    }
}
