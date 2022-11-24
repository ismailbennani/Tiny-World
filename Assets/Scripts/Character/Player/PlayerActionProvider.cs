﻿using System;
using Input;
using Map;
using Map.Tile;
using UnityEngine;

namespace Character.Player
{
    public class PlayerActionProvider : MonoBehaviour
    {
        private Vector2Int? _lastKnownPlayerPosition;

        private GatherResourceController _playerGatherResourceController;

        void Start()
        {
            PlayerController playerController = PlayerController.Instance;
            
            playerController.gameObject.TryGetComponent(out _playerGatherResourceController);
        }

        void Update()
        {
            GameState state = GameStateManager.Current;
            if (!state || state.map == null || state.player == null)
            {
                return;
            }

            AddInteractCallback(state);
        }

        private void AddInteractCallback(GameState state)
        {
            Vector2Int playerPosition = state.player.playerTile;
            if (playerPosition == _lastKnownPlayerPosition)
            {
                return;
            }

            _lastKnownPlayerPosition = playerPosition;
            TileState tile = state.map.GetTile(playerPosition);

            if (tile.HasResource)
            {
                GameInputCallback callback = GetCollectResourceCallback(tile);
                
                if (callback != null)
                {
                    GameInputCallbackManager.Instance.Register(GameInputType.Interact, this, callback);
                }
            }
            else
            {
                GameInputCallbackManager.Instance.Unregister(GameInputType.Interact, this);
            }
        }

        private GameInputCallback GetCollectResourceCallback(TileState tile)
        {
            if (!_playerGatherResourceController)
            {
                return null;
            }
            
            switch (tile.config.resource)
            {
                case ResourceType.None:
                    Debug.LogWarning("Cannot mine tile with no resource");
                    return null;
                case ResourceType.Wood:
                    return _playerGatherResourceController.allowChop ? new GameInputCallback("Chop", _playerGatherResourceController.Chop) : null;
                case ResourceType.Stone:
                    return _playerGatherResourceController.allowMine ? new GameInputCallback("Mine", _playerGatherResourceController.Mine) : null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tile.config.resource), tile.config.resource, null);
            }
        }
    }
}