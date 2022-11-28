using Items;
using UI.Theme;

namespace UI.Inventory
{
    public class UIInventoryGridItem : UIButton
    {
        public void SetItem(Item newItem, int newCount)
        {
            SetImage(newItem.sprite);
            SetText(newCount.ToString());
        }

        protected override UIButtonTheme GetTheme(UITheme theme)
        {
            return theme.button;
        }
    }
}
