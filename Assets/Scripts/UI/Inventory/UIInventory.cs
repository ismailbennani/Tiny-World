using System.Collections;
using Character.Inventory;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class UIInventory : MonoBehaviour
    {
        public UIInventory Instance { get; private set; }

        public UIInventoryTheme defaultTheme;
        public UIInventoryTheme currentTheme;

        [Header("Main window")]
        public Image panel;
        public Image closeButton;
        public Image closeButtonIcon;
        public TextMeshProUGUI title;

        [Header("Grid")]
        public Image gridPanel;
        public UIInventoryGrid grid;

        private InventoryState _inventoryState;

        void OnEnable()
        {
            Instance = this;

            StartCoroutine(ApplyThemeWhenReady());
        }

        public void SetTheme(UIInventoryTheme theme)
        {
            if (!theme)
            {
                if (defaultTheme)
                {
                    SetTheme(defaultTheme);
                }

                return;
            }

            panel.sprite = theme.panel;
            closeButton.sprite = theme.closeButton;
            closeButtonIcon.sprite = theme.closeButtonIcon;
            title.font = theme.title.font;
            title.color = theme.title.color;

            gridPanel.sprite = theme.gridPanel;
            grid.GridItemDefaultPanel = theme.gridItemPanel;
            grid.GridItemSelectedPanel = theme.gridItemSelectedPanel;
            grid.GridItemCountFont = theme.gridItemCount.font;
            grid.GridItemCountColor = theme.gridItemCount.color;

            currentTheme = theme;
        }

        public void Open()
        {
            gameObject.SetActive(true);

            UpdateInventory();

            _inventoryState?.onChange.AddListener(OnInventoryChange);
        }

        public void Close()
        {
            gameObject.SetActive(false);

            _inventoryState?.onChange.RemoveListener(OnInventoryChange);
        }

        public void Toggle()
        {
            if (gameObject.activeInHierarchy)
            {
                Close();
            }
            else
            {
                Open();
            }
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

        private IEnumerator ApplyThemeWhenReady()
        {
            while (!GameStateManager.Config)
            {
                yield return null;
            }

            SetTheme(GameStateManager.Config.inventoryTheme);
        }
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(UIInventory))]
    public class UIInventoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Apply default theme"))
            {
                UIInventory inventory = target as UIInventory;
                if (!inventory)
                {
                    return;
                }

                if (!inventory.defaultTheme)
                {
                    return;
                }

                inventory.SetTheme(inventory.defaultTheme);
            }

            if (GUILayout.Button("Save current to default theme"))
            {
                UIInventory inventory = target as UIInventory;
                if (!inventory)
                {
                    return;
                }

                UIInventoryTheme defaultTheme = inventory.defaultTheme;
                if (!defaultTheme)
                {
                    return;
                }

                defaultTheme.panel = inventory.panel.sprite;
                defaultTheme.closeButton = inventory.closeButton.sprite;
                defaultTheme.closeButtonIcon = inventory.closeButtonIcon.sprite;
                defaultTheme.title.font = inventory.title.font;
                defaultTheme.title.color = inventory.title.color;

                defaultTheme.gridPanel = inventory.gridPanel.sprite;
                defaultTheme.gridItemPanel = inventory.grid.GridItemDefaultPanel;
                defaultTheme.gridItemSelectedPanel = inventory.grid.GridItemSelectedPanel;
                defaultTheme.gridItemCount.font = inventory.grid.GridItemCountFont;
                defaultTheme.gridItemCount.color = inventory.grid.GridItemCountColor;
            }
        }
    }

    #endif
}
