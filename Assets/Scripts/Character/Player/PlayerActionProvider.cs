using System;
using Input;
using Map;
using Map.Tile;
using UnityEngine;

namespace Character.Player
{
    public class PlayerActionProvider : MonoBehaviour
    {
        private Vector2Int? _lastKnownPlayerPosition;

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
                GameInputCallback callback = GetMineResourceCallback(tile);
                GameInputCallbackManager.Instance.Register(GameInputType.Interact, this, callback);
            }
            else
            {
                GameInputCallbackManager.Instance.Unregister(GameInputType.Interact, this);
            }
        }

        private GameInputCallback GetMineResourceCallback(TileState tile)
        {
            string name = tile.config.resource switch
            {
                ResourceType.None => throw new InvalidOperationException("Cannot mine tile with no resource"),
                ResourceType.Wood => "Chop",
                ResourceType.Stone => "Mine",
                _ => throw new ArgumentOutOfRangeException(nameof(tile.config.resource), tile.config.resource, null)
            };

            return new GameInputCallback(name, () => Debug.Log($"MINE at {tile.position}"));
        }
    }
}
