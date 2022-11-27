using UnityEngine;

namespace Items
{
    public class GameItem: MonoBehaviour
    {
        public ItemState state;
        public new Rigidbody rigidbody;

        private GameObject _itemObject;
        private bool _hidden;
        
        void Update()
        {
            if (state == null)
            {
                return;
            }

            state.position = transform.position;

            if (state.position.y < -10)
            {
                GameStateManager.Current.map.RemoveItem(state);
            }
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

            if (newState.newlySpawned && rigidbody)
            {
                Vector3 force = Random.onUnitSphere;
                force.y = 1;
                rigidbody.AddForce(force, ForceMode.Impulse);

                newState.newlySpawned = false;
            }

            GameConfig gameState = GameStateManager.Config;
            GameObject prefab = gameState.items.GetPrefab(newState);
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
