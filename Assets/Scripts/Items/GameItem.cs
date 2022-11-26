using UnityEngine;

namespace Items
{
    public class GameItem: MonoBehaviour
    {
        public ItemState state;
        public bool animateOnStart;

        private GameObject _itemObject;
        private bool _hidden;

        void Start()
        {
            if (state != null && animateOnStart)
            {
                transform.Translate(Random.Range(-0.5f, 0.5f), 1, Random.Range(-0.5f, 0.5f));
                state.position = transform.position;
                animateOnStart = false;
            }
        }
        
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
