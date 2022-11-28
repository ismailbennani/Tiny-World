using UI.Theme;

namespace UI.MainMenu
{
    public class UIMenuButton: UIButton
    {
        protected override UIButtonTheme GetTheme(UITheme theme)
        {
            return theme.button;
        }
    }
}
