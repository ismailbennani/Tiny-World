using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIMainMenu : UIWindow
    {
        private const string InventoryButtonName = "InventoryMenuButton";
        
        [SerializeField]
        private string _currentSelection;

        protected override void RegisterAdditionalCallbacks()
        {
            Button inventoryMenuButton = root.rootVisualElement.Q<Button>(InventoryButtonName);
            inventoryMenuButton.clicked += OpenInventory;
        }

        protected override void OnFocus()
        {
            Button toFocus = root.rootVisualElement.Query<Button>(_currentSelection).First()
                             ?? root.rootVisualElement.Query<Button>().Enabled().First();

            toFocus?.Focus();
        }

        private void OpenInventory()
        {
            _currentSelection = InventoryButtonName;
            Debug.Log("INVENTORY");
        }
    }
}
