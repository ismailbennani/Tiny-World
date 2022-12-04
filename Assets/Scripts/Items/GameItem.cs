using Map;
using Map.Chunk;
using UnityEngine;
using Utils;

namespace Items
{
    public class GameItem: MonoBehaviour
    {
        public string guid;
        public Vector2Int chunk;
        public bool hidden;
        
        public new Rigidbody rigidbody;

        private GameObject _itemObject;
        private HighlightableGameObject _highlightable;

        void Start()
        {
            Unhighlight();
        }
        
        void Update()
        {
            if (guid == null)
            {
                return;
            }

            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            ChunkState chunkState = gameState.map.GetChunk(chunk);
            ItemState state = chunkState?.GetItem(guid);
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
            if (!hidden && guid == newState?.guid)
            {
                return;
            }

            if (newState == null)
            {
                gameObject.SetActive(false);
                hidden = true;
                return;
            }
            
            guid = newState.guid;
            chunk = newState.chunk;
            
            gameObject.SetActive(true);
            hidden = false;
            
            if (_itemObject)
            {
                Destroy(_itemObject);
            }

            transform.position = newState.position;

            if (newState.newlySpawned && rigidbody)
            {
                transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                
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
                Debug.LogWarning($"Missing highlight for item {guid} at {chunk}");
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
