using System;
using System.Collections.Generic;
using Character.Inventory;
using UnityEngine.UIElements;

namespace UI
{
    public class UIInventory: UIWindow
    {
        public VisualTreeAsset gridItemTemplate;
        
        private VisualElement _itemContainer;
        private readonly List<VisualElement> _inventoryItems = new();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!gridItemTemplate)
            {
                throw new InvalidOperationException("Grid item template not set");
            }
            
            _itemContainer = root.rootVisualElement.Q("InventoryItemContainer");
        }
        
        protected override void RegisterAdditionalCallbacks()
        {
            
        }

        protected override void Load()
        {
            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            InventoryState inventory = gameState.player?.inventoryState;
            if (inventory == null)
            {
                return;
            }

            // RESET
            // TODO: don't destroy and recreate elements
            foreach (VisualElement inventoryItem in _inventoryItems)
            {
                inventoryItem.RemoveFromHierarchy();
            }

            _inventoryItems.Clear();

            foreach (InventoryLine line in inventory.lines)
            {
                TemplateContainer newInventoryItemTemplate = gridItemTemplate.CloneTree();
                newInventoryItemTemplate.Q<Label>("Count").text = line.count.ToString();

                if (line.item.sprite)
                {
                    VisualElement image = newInventoryItemTemplate.Q<VisualElement>("Image");
                    image.style.backgroundImage = new StyleBackground(line.item.sprite);
                }

                _itemContainer.Add(newInventoryItemTemplate);
                _inventoryItems.Add(newInventoryItemTemplate);
            }
        }

        protected override void OnFocus()
        {
            if (_inventoryItems.Count > 0)
            {
                _itemContainer.Query<Button>().First().Focus();
            }
            else if (CloseButton != null)
            {
                CloseButton.Focus();
            }
        }
    }
}
