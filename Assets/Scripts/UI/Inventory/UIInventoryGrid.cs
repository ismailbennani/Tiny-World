using System.Collections.Generic;
using Character.Inventory;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public class UIInventoryGrid : MonoBehaviour
    {
        public UIInventoryGridItem gridItemPrefab;

        public Sprite GridItemDefaultPanel { get => gridItemPrefab.DefaultPanel; set => gridItemPrefab.DefaultPanel = value; }
        public Sprite GridItemSelectedPanel { get => gridItemPrefab.SelectedPanel; set => gridItemPrefab.SelectedPanel = value; }
        public TMP_FontAsset GridItemCountFont { get => gridItemPrefab.ItemCountFont; set => gridItemPrefab.ItemCountFont = value; }
        public Color GridItemCountColor { get => gridItemPrefab.ItemCountColor; set => gridItemPrefab.ItemCountColor = value; }

        private List<UIInventoryGridItem> _gridItems;

        private void OnEnable()
        {
            gridItemPrefab.gameObject.SetActive(false);
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

                _gridItems.Add(newGridItem);
            }
        }
    }
}
