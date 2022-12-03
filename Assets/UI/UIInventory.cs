﻿using System;
using System.Collections.Generic;
using Character.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIInventory : UIWindow
    {
        public VisualTreeAsset gridItemTemplate;

        private VisualElement _itemContainer;
        private readonly List<VisualElement> _inventoryItems = new();

        private VisualElement _descriptionRoot;
        private Label _descriptionTitle;
        private Label _descriptionBody;

        [Header("Runtime")]
        [SerializeField]
        private int currentFocus;

        [SerializeField]
        private bool saveFocusedButton;
        private InventoryState _inventory;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!gridItemTemplate)
            {
                throw new InvalidOperationException("Grid item template not set");
            }

            _itemContainer = root.rootVisualElement.Q("InventoryItemContainer");

            _descriptionRoot = root.rootVisualElement.Q("DescriptionRoot");
            _descriptionTitle = root.rootVisualElement.Q<Label>("DescriptionTitle");
            _descriptionBody = root.rootVisualElement.Q<Label>("DescriptionBody");
        }

        protected override void RegisterAdditionalCallbacks()
        {
        }

        protected override void OnOpen()
        {
            _descriptionRoot.visible = false;

            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            _inventory = gameState.player?.inventoryState;
            if (_inventory == null)
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

            for (int index = 0; index < _inventory.lines.Count; index++)
            {
                int indexCopy = index;
                
                InventoryLine line = _inventory.lines[index];
                TemplateContainer newInventoryItemTemplate = gridItemTemplate.CloneTree();
                Button button = newInventoryItemTemplate.Q<Button>();
                button.RegisterCallback<FocusEvent>(
                    _ =>
                    {
                        _descriptionRoot.visible = true;
                        _descriptionTitle.text = line.item.itemName;
                        _descriptionBody.text = line.item.itemDescription;

                        if (saveFocusedButton)
                        {
                            currentFocus = indexCopy;
                        }
                    }
                );
                button.clicked += () =>
                {
                    Rect rect = button.worldBound;
                    
                    UIMenusManager.Instance.OpenDropdown(
                        new[]
                        {
                            new UIDropdownChoice("Drop", () => Inventory.Drop(_inventory, line.item, 1, indexCopy)),
                            new UIDropdownChoice("Drop all", () => Inventory.Drop(_inventory, line.item, line.count, indexCopy)),
                        },
                        new Vector2(rect.x + rect.width, rect.y - rect.height / 2)
                    );
                };

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

        protected override void OnFocusIn()
        {
            saveFocusedButton = true;
            
            if (_inventoryItems.Count > 0)
            {
                currentFocus = Mathf.Clamp(currentFocus, 0, _inventoryItems.Count - 1);
                _itemContainer.Query<Button>().AtIndex(currentFocus).Focus();
            }
            else if (CloseButton != null)
            {
                CloseButton.Focus();
            }
        }
        
        protected override void OnFocusOut()
        {
            saveFocusedButton = false;
        }

        protected override void OnClose()
        {
            _descriptionRoot.visible = false;
        }
    }
}
