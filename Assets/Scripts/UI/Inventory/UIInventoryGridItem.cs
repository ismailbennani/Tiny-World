using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class UIInventoryGridItem : MonoBehaviour
    {
        public Image defaultPanel;
        public Image selectedPanel;
        public Image itemImage;
        public TextMeshProUGUI itemCount;

        public Item item;
        public int count;
        public bool selected;

        public Sprite DefaultPanel { get => defaultPanel.sprite; set => defaultPanel.sprite = value; }
        public Sprite SelectedPanel { get => selectedPanel.sprite; set => selectedPanel.sprite = value; }
        public TMP_FontAsset ItemCountFont { get => itemCount.font; set => itemCount.font = value; }
        public Color ItemCountColor { get => itemCount.color; set => itemCount.color = value; }

        void Awake()
        {
            SetItem(item);
            SetCount(count);
            
            if (selected)
            {
                Select();
            }
            else
            {
                Unselect();
            }
        }
        
        public void Select()
        {
            defaultPanel.gameObject.SetActive(false);
            selectedPanel.gameObject.SetActive(true);

            selected = true;
        }
        
        public void Unselect()
        {
            defaultPanel.gameObject.SetActive(true);
            selectedPanel.gameObject.SetActive(false);

            selected = false;
        }

        public void SetItem(Item newItem)
        {
            if (newItem && newItem.sprite)
            {
                itemImage.sprite = newItem.sprite;
                itemImage.gameObject.SetActive(true);
            }
            else
            {
                itemImage.gameObject.SetActive(false);
            }

            item = newItem;
        }

        public void SetCount(int newCount)
        {
            itemCount.SetText(newCount.ToString("N0"));

            count = newCount;
        }
    }
}
