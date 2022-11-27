using UnityEngine;

namespace Items
{
    public class GameItem: MonoBehaviour
    {
        public ItemState state;

        private GameObject _itemObject;
        private bool _hidden;
        
        void Update()
        {
            if (state == null)
            {
                return;
            }

            state.position = transform.position;
        }
        
        public void Set(ItemState newState)
        {
            if (newState != null && newState.Equals(state) || newState == null && state == null)
            {
                return;
            }

            if (_itemObject)
            {
                Destroy(_itemObject);
            }

            state = newState;

            if (newState == null)
            {
                return;
            }

            transform.position = newState.position;

            GameConfig gameState = GameStateManager.Config;
            GameObject prefab = gameState.items.GetPrefab(newState.item);
            if (!prefab)
            {
                return;
            }

            _itemObject = Instantiate(prefab, transform);
        }

        public void Hide()
        {
            Set(null);
        }
    }
}
