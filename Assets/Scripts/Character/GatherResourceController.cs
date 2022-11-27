using Map;
using Map.Tile;
using UnityEngine;

namespace Character
{
    public class GatherResourceController : MonoBehaviour
    {
        public bool allowMine;
        public bool allowChop;
        
        // Loot is the generic animation
        private int _animIDLoot;
        private int _animIDMine;
        private int _animIDChop;

        private Animator _animator;
        private ThirdPersonController _thirdPersonController;

        void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _animIDLoot = Animator.StringToHash("Loot");
            _animIDMine = Animator.StringToHash("Mine");
            _animIDChop = Animator.StringToHash("Chop");

            TryGetComponent(out _thirdPersonController);
            if (_thirdPersonController)
            {
                _thirdPersonController.onMoveStart.AddListener(CancelGather);
            }
        }

        public void Loot()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.character.tile);
            if (!tile.IsLootable)
            {
                return;
            }
            
            _animator.SetBool(_animIDLoot, true);
        }

        public void Mine()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.character.tile);
            if (tile.config.tileResource != TileResourceType.Rock || !tile.IsLootable)
            {
                return;
            }
            
            _animator.SetBool(_animIDMine, true);
        }

        public void Chop()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.character.tile);
            if (tile.config.tileResource != TileResourceType.Tree || !tile.IsLootable)
            {
                return;
            }
            
            _animator.SetBool(_animIDMine, true);
        }

        public void CancelGather()
        {
            _animator.SetBool(_animIDLoot, false);
            _animator.SetBool(_animIDMine, false);
            _animator.SetBool(_animIDChop, false);
        }

        void OnGather()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                Debug.LogWarning($"Invalid state: {nameof(GameState)} cannot be null");
                return;
            }

            GameMap map = GameMap.Instance;
            if (!map)
            {
                Debug.LogWarning($"Invalid state: {nameof(GameMap)} cannot be null");
                return;
            }

            TileState tile = state.map.GetTile(state.character.tile);
            
            MapTile mapTile = map.GetTile(tile);
            if (!mapTile)
            {
                Debug.LogWarning($"Invalid state: tile {tile.position} is not rendered");
                return;
            }

            if (!tile.IsLootable)
            {
                Debug.LogWarning($"Invalidstate: tile {tile.position} is not lootable");
                CancelGather();
            }

            tile.Loot(1);

            if (!tile.IsLootable)
            {
                CancelGather();
            }
            
        }
    }
}
