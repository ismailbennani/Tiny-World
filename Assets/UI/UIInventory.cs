using System;
using System.Collections.Generic;
using System.Linq;
using Character.Inventory;
using Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIInventory : UIWindow
    {
        public VisualTreeAsset gridItemTemplate;

        private VisualElement _itemContainer;
        private readonly List<InventoryItemElement> _inventoryItems = new();

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
            foreach (InventoryItemElement inventoryItem in _inventoryItems.ToList())
            {
                inventoryItem.Remove();
            }

            for (int index = 0; index < _inventory.lines.Count; index++)
            {
                InventoryLine line = _inventory.lines[index];
                TemplateContainer itemElement = gridItemTemplate.CloneTree();

                InventoryItemElement inventoryItemElement = InventoryItemElement.Create(this, line, itemElement, index);

                _itemContainer.Add(itemElement);
                _inventoryItems.Add(inventoryItemElement);
            }
        }

        protected override void OnFocusIn()
        {
            saveFocusedButton = true;

            if (_inventoryItems.Count > 0)
            {
                currentFocus = Mathf.Clamp(currentFocus, 0, _inventoryItems.Count - 1);
                _itemContainer.Query<Button>().AtIndex(currentFocus)?.Focus();
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

        private void ShowDescription(Item item)
        {
            _descriptionRoot.visible = true;
            _descriptionTitle.text = item.itemName;
            _descriptionBody.text = item.itemDescription;
        }

        private void SetCurrentFocus(int index)
        {
            if (saveFocusedButton)
            {
                currentFocus = index;
            }
        }

        private void RemoveItem(InventoryItemElement item)
        {
            _inventoryItems.Remove(item);
        }

        private class InventoryItemElement
        {
            public readonly UIInventory Inventory;
            public readonly VisualElement Element;
            public readonly InventoryLine Line;

            private InventoryItemElement(UIInventory inventory, InventoryLine line, VisualElement element)
            {
                Inventory = inventory;
                Element = element;
                Line = line;
            }

            private void Update()
            {
                if (Line.count <= 0)
                {
                    Remove();
                }
                else
                {
                    Label label = Element.Q<Label>("Count");
                    label.text = Line.count.ToString();
                }
            }

            public void Remove()
            {
                Element.RemoveFromHierarchy();
                Line.onChange.RemoveListener(Update);
                Inventory.RemoveItem(this);
            }

            public static InventoryItemElement Create(UIInventory inventory, InventoryLine line, TemplateContainer itemElement, int index)
            {
                InventoryItemElement result = new(inventory, line, itemElement);

                line.onChange.AddListener(result.Update);

                Button button = itemElement.Q<Button>();
                button.RegisterCallback<FocusEvent>(
                    _ =>
                    {
                        inventory.ShowDescription(line.item);
                        inventory.SetCurrentFocus(index);
                    }
                );
                button.clicked += () =>
                {
                    Rect rect = button.worldBound;

                    UIMenusManager.Instance.OpenDropdown(
                        new[]
                        {
                            new UIDropdownChoice("Drop", () => Character.Inventory.Inventory.Drop(inventory._inventory, line.item, 1, index)),
                            new UIDropdownChoice("Drop all", () => Character.Inventory.Inventory.Drop(inventory._inventory, line.item, line.count, index)),
                        },
                        new Vector2(rect.x + rect.width, rect.y - 10)
                    );
                };

                itemElement.Q<Label>("Count").text = line.count.ToString();

                if (line.item.sprite)
                {
                    VisualElement image = itemElement.Q<VisualElement>("Image");
                    image.style.backgroundImage = new StyleBackground(line.item.sprite);
                }

                return result;
            }
        }
    }
}
