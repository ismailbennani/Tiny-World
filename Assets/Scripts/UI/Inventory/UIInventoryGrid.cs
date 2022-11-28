using System.Collections.Generic;
using Character.Inventory;
using UI.Theme;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Inventory
{
    public class UIInventoryGrid : MonoBehaviour
    {
        public UIInventoryGridItem gridItemPrefab;

        private List<UIInventoryGridItem> _gridItems = new();

        private void OnEnable()
        {
            gridItemPrefab.gameObject.SetActive(false);
        }

        public void OnFocus()
        {
            if (_gridItems.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(_gridItems[0].gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
            }
        }

        public void UpdateItems(InventoryState inventoryState)
        {
            if (_gridItems != null)
            {
                foreach (UIInventoryGridItem gridItem in _gridItems)
                {
                    Destroy(gridItem.gameObject);
                }
            }

            _gridItems = new List<UIInventoryGridItem>();

            foreach (InventoryLine line in inventoryState.lines)
            {
                UIInventoryGridItem newGridItem = Instantiate(gridItemPrefab, transform);
                newGridItem.SetItem(line.item);
                newGridItem.SetCount(line.count);

                newGridItem.gameObject.SetActive(true);
                
                _gridItems.Add(newGridItem);
            }
        }

        public void SetTheme(UITheme theme)
        {
            foreach (UIInventoryGridItem item in _gridItems)
            {
                item.SetTheme(theme);
            }
        }

        public void SaveTheme(UITheme theme)
        {
            if (_gridItems.Count > 0)
            {
                _gridItems[0].SaveTheme(theme);
            }
        }
    }
}
