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

        private ResourceType _currentResource;

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
            _currentResource = ResourceType.Stone;
            _animator.SetBool(_animIDMine, true);
        }
        
        public void Chop()
        {
            _currentResource = ResourceType.Wood;
            _animator.SetBool(_animIDMine, true);
        }

        public void CancelGather()
        {
            _currentResource = ResourceType.None;
            _animator.SetBool(_animIDMine, false);
            _animator.SetBool(_animIDChop, false);
        }

        void OnGather()
        {
            if (_currentResource == ResourceType.None)
            {
                return;
            }
            
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }

            Debug.Log($"GATHER {_currentResource} on {state.player.playerTile}");
        }
    }
}
