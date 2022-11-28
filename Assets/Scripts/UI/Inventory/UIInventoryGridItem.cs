using Items;
using UI.Theme;

namespace UI.Inventory
{
    public class UIInventoryGridItem : UIButton
    {
        private Item _item;

        void Start()
        {
            SetItem(_item);
        }

        public void SetItem(Item newItem)
        {
            SetImage(newItem.sprite);

            _item = newItem;
        }

        public void SetCount(int newCount)
        {
            SetText(newCount.ToString());
        }

        protected override UIButtonTheme GetTheme(UITheme theme)
        {
            return theme.button;
        }
    }
}
