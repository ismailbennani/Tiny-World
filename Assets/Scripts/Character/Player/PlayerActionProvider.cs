using Input;
using Map;
using Map.Tile;
using UnityEngine;

namespace Character.Player
{
    public class PlayerActionProvider : MonoBehaviour
    {
        private const int LootTileChannel = 0;
        private const int TakeItemChannel = 1;

        private GatherResourceController _playerGatherResourceController;
        private CharacterItemsDetector _playerItemsDetector;

        private Vector2Int? _currentTileTarget;
        private string _currentItemTarget;

        void Update()
        {
            TryGetCharacterItemsDetectorIfNecessary();
            TryGetGatherResourceControllerIfNecessary();

            GameState state = GameStateManager.Current;
            if (!state || state.map == null || state.player == null)
            {
                return;
            }

            UpdateTileInteract(state);
            UpdateItemInteract();
        }

        private void UpdateItemInteract()
        {
            if (_playerItemsDetector && _playerItemsDetector.closestItem)
            {
                if (_currentItemTarget == _playerItemsDetector.closestItem.state.guid)
                {
                    return;
                }

                GameInputCallbackManager.Instance.Register(
                    GameInputType.Interact,
                    this,
                    new GameInputCallback(
                        $"Take {_playerItemsDetector.closestItem.state.item.itemName}",
                        () => Debug.Log($"TAKE {_playerItemsDetector.closestItem.state.item.itemName}"),
                        10
                    ),
                    TakeItemChannel
                );

                _currentItemTarget = _playerItemsDetector.closestItem.state.guid;
            }
            else
            {
                GameInputCallbackManager.Instance.Unregister(GameInputType.Interact, this, TakeItemChannel);
            }
        }

        private void UpdateTileInteract(GameState state)
        {
            Vector2Int playerPosition = state.player.tile;

            if (_currentTileTarget == playerPosition)
            {
                return;
            }

            if (_currentTileTarget.HasValue)
            {
                TileState lastTile = state.map.GetTile(_currentTileTarget.Value);
                lastTile.onDepleted.RemoveListener(UnregisterLootInteract);
                _currentTileTarget = null;
            }

            TileState tile = state.map.GetTile(playerPosition);
            if (tile.IsLootable && _playerGatherResourceController)
            {
                GameInputCallback callback;
                switch (tile.config.tileResource)
                {
                    case TileResourceType.Tree:
                        callback = _playerGatherResourceController.allowChop ? new GameInputCallback("Chop", _playerGatherResourceController.Chop) : null;
                        break;
                    case TileResourceType.Rock:
                        callback = _playerGatherResourceController.allowMine ? new GameInputCallback("Mine", _playerGatherResourceController.Mine) : null;
                        break;
                    default:
                        callback = _playerGatherResourceController.allowChop ? new GameInputCallback("Loot", _playerGatherResourceController.Loot) : null;
                        break;
                }

                if (callback != null)
                {
                    GameInputCallbackManager.Instance.Register(GameInputType.Interact, this, callback, LootTileChannel);
                    tile.onDepleted.AddListener(UnregisterLootInteract);
                    return;
                }
            }
            
            _currentTileTarget = playerPosition;

            UnregisterLootInteract();
        }

        private void UnregisterLootInteract()
        {
            GameInputCallbackManager.Instance.Unregister(GameInputType.Interact, this, LootTileChannel);
        }

        private void TryGetGatherResourceControllerIfNecessary()
        {
            if (_playerGatherResourceController)
            {
                return;
            }

            PlayerController playerController = PlayerController.Instance;
            if (!playerController)
            {
                return;
            }

            _playerGatherResourceController = playerController.gameObject.GetComponent<GatherResourceController>();
        }

        private void TryGetCharacterItemsDetectorIfNecessary()
        {
            if (_playerItemsDetector)
            {
                return;
            }

            PlayerController playerController = PlayerController.Instance;
            if (!playerController)
            {
                return;
            }

            _playerItemsDetector = playerController.gameObject.GetComponentInChildren<CharacterItemsDetector>();
            _playerItemsDetector.onClosestItemChange.AddListener(_ => UpdateItemInteract());
        }
    }
}
