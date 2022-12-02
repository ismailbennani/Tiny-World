using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIMainMenu : UIWindow
    {
        private const string InventoryButtonName = "InventoryMenuButton";
        
        [SerializeField]
        private string currentSelection;

        protected override void RegisterAdditionalCallbacks()
        {
            Button inventoryMenuButton = root.rootVisualElement.Q<Button>(InventoryButtonName);
            inventoryMenuButton.clicked += OpenInventory;
        }

        protected override void OnOpen()
        {
            Button toFocus = root.rootVisualElement.Query<Button>(currentSelection).First()
                             ?? root.rootVisualElement.Query<Button>().Enabled().First();

            toFocus?.Focus();
        }

        protected override void OnClose()
        {
        }

        private void OpenInventory()
        {
            currentSelection = InventoryButtonName;

            UIMenusManager.Instance.OpenInventory();
        }
    }
}
