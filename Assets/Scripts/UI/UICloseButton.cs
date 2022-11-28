using UI.Theme;

namespace UI
{
    public class UICloseButton: UIButton
    {
        protected override UIButtonTheme GetTheme(UITheme theme)
        {
            return theme.closeButton;
        }

        protected override void OnSetTheme(UITheme theme)
        {
            base.OnSetTheme(theme);

            SetImage(theme.closeButtonIcon);
        }

        protected override void OnSaveTheme(UITheme theme)
        {
            base.OnSaveTheme(theme);

            if (image)
            {
                theme.cursor = image.sprite;
            }
        }
    }
}
