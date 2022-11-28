namespace UI.Menu
{
    public class UIMenu
    {
        public static UIMenu Instance { get; private set; }

        void OnEnable()
        {
            Instance = this;
        }

        public void Toggle()
        {
            
        }
    }
}
