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
        public Transform itemViewParent;
        public TextMeshProUGUI itemCount;

        public Sprite DefaultPanel { get => defaultPanel.sprite; set => defaultPanel.sprite = value; }
        public Sprite SelectedPanel { get => selectedPanel.sprite; set => selectedPanel.sprite = value; }
        public TMP_FontAsset ItemCountFont { get => itemCount.font; set => itemCount.font = value; }
        public Color ItemCountColor { get => itemCount.color; set => itemCount.color = value; }

        private GameObject _itemPrefab;

        void Start()
        {
            SetItem(null);
            Unselect();
        }
        
        public void Select()
        {
            defaultPanel.gameObject.SetActive(false);
            selectedPanel.gameObject.SetActive(true);
        }
        
        public void Unselect()
        {
            defaultPanel.gameObject.SetActive(false);
            selectedPanel.gameObject.SetActive(true);
        }

        public void SetItem(Item item)
        {
            if (_itemPrefab)
            {
                Destroy(_itemPrefab);
            }

            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            ItemsRuntimeConfig itemsConfig = gameState.itemsConfig;
            if (!itemsConfig)
            {
                return;
            }
            
            GameObject itemPrefab = itemsConfig.GetPrefab(item);
            if (!itemPrefab)
            {
                return;
            }
            
            _itemPrefab = Instantiate(itemPrefab, itemViewParent);
        }

        public void SetCount(int lineCount)
        {
            itemCount.SetText(lineCount.ToString("N0"));
        }
    }
}
