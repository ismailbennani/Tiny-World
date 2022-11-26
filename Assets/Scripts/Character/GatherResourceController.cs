using Map;
using Map.Tile;
using UnityEngine;

namespace Character
{
    public class GatherResourceController : MonoBehaviour
    {
        public bool allowMine;
        public bool allowChop;
        
        private int _animIDMine;
        private int _animIDChop;

        private Animator _animator;
        private ThirdPersonController _thirdPersonController;

        private TileResourceType _currentTileResource;

        void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _animIDMine = Animator.StringToHash("Mine");
            _animIDChop = Animator.StringToHash("Chop");

            TryGetComponent(out _thirdPersonController);
            if (_thirdPersonController)
            {
                _thirdPersonController.onMoveStart.AddListener(CancelGather);
            }
        }

        public void Mine()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.player.playerTile);
            if (tile.config.tileResource != TileResourceType.Rock || !tile.HasResource)
            {
                return;
            }
            
            _currentTileResource = TileResourceType.Rock;
            _animator.SetBool(_animIDMine, true);
            
        }
        
        public void Chop()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.player.playerTile);
            if (tile.config.tileResource != TileResourceType.Tree || !tile.HasResource)
            {
                return;
            }
            
            _currentTileResource = TileResourceType.Tree;
            _animator.SetBool(_animIDMine, true);
        }

        public void CancelGather()
        {
            _currentTileResource = TileResourceType.None;
            _animator.SetBool(_animIDMine, false);
            _animator.SetBool(_animIDChop, false);
        }

        void OnGather()
        {
            if (_currentTileResource == TileResourceType.None)
            {
                Debug.LogWarning($"Invalid state: {nameof(_currentTileResource)} cannot be {TileResourceType.None}");
                return;
            }
            
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

            TileState tile = state.map.GetTile(state.player.playerTile);
            
            MapTile mapTile = map.GetTile(tile);
            if (!mapTile)
            {
                Debug.LogWarning($"Invalid state: tile {tile.position} is not rendered");
                return;
            }

            tile.ConsumeResource(1);

            if (!tile.HasResource)
            {
                CancelGather();
            }
            
        }
    }
}
