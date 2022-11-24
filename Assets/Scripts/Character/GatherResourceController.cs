﻿using Map;
using Map.Tile;
using Resource;
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
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.player.playerTile);
            if (tile.config.resource != ResourceType.Stone || !tile.HasResource)
            {
                return;
            }
            
            _currentResource = ResourceType.Stone;
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
            if (tile.config.resource != ResourceType.Wood || !tile.HasResource)
            {
                return;
            }
            
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

            TileState tile = state.map.GetTile(state.player.playerTile);
            
            int actualConsumption = tile.ConsumeResource(1);
            state.resource.Produce(_currentResource, actualConsumption);

            if (!tile.HasResource)
            {
                CancelGather();
            }
        }
    }
}
