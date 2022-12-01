using UnityEngine.UIElements;

namespace UI
{
    public class UIInventory: UIWindow
    {
        private const string InventoryName = "InventoryMenu";
        
        protected virtual VisualElement GetVisualElement()
        {
            return root.rootVisualElement.Q(InventoryName);
        }

        protected override void RegisterAdditionalCallbacks()
        {
            
        }

        protected override void OnFocus()
        {
            
        }
    }
}
