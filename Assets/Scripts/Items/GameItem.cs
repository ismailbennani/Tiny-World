using UnityEngine;
using Utils;

namespace Items
{
    public class GameItem: MonoBehaviour
    {
        public ItemState state;
        public new Rigidbody rigidbody;

        private GameObject _itemObject;
        private HighlightableGameObject _highlightable;
        private bool _hidden;

        void Start()
        {
            Unhighlight();
        }
        
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
            if (!_hidden && newState != null && newState.Equals(state) || newState == null && state == null)
            {
                return;
            }

            state = newState;

            if (newState == null)
            {
                gameObject.SetActive(false);
                _hidden = true;
                return;
            }
            
            gameObject.SetActive(true);
            _hidden = false;
            
            if (_itemObject)
            {
                Destroy(_itemObject);
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

        public void Highlight()
        {
            if (!_highlightable)
            {
                _highlightable = GetComponentInChildren<HighlightableGameObject>();
            }
            
            if (_highlightable)
            {
                _highlightable.Highlight();
            }
            else
            {
                Debug.LogWarning($"Missing highlight for item {state.item}");
            }
        }

        public void Unhighlight()
        {
            if (!_highlightable)
            {
                _highlightable = GetComponentInChildren<HighlightableGameObject>();
            }
            
            if (_highlightable)
            {
                _highlightable.Unhighlight();
            }
        }

        public void Hide()
        {
            Set(null);
        }
    }
}
