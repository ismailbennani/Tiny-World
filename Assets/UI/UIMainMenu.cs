using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIMainMenu : UIWindow
    {
        private const string InventoryButtonName = "InventoryMenuButton";
        private const string SettingsButtonName = "SettingsMenuButton";

        [SerializeField]
        private int currentFocus;

        protected override void RegisterAdditionalCallbacks()
        {
            Button inventoryMenuButton = root.rootVisualElement.Q<Button>(InventoryButtonName);
            inventoryMenuButton.clicked += UIMenusManager.Instance.OpenInventory;
            
            List<Button> buttons = root.rootVisualElement.Query<Button>().ToList();
            for (int i = 0; i < buttons.Count; i++)
            {
                int iCopy = i;
                buttons[i].RegisterCallback<FocusEvent>(_ => currentFocus = iCopy);
            }
        }

        protected override void OnOpen()
        {
        }

        protected override void OnFocus()
        {
            UQueryBuilder<Button> buttons = root.rootVisualElement.Query<Button>();
            Button toFocus = buttons.AtIndex(currentFocus) ?? buttons.Enabled().First();

            toFocus?.Focus();
        }

        protected override void OnClose()
        {
        }
    }
}
