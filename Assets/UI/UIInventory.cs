namespace UI
{
    public class UIInventory: UIWindow
    {
        protected override void RegisterAdditionalCallbacks()
        {
            
        }

        protected override void OnFocus()
        {
            root.rootVisualElement.Focus();
        }
    }
}
