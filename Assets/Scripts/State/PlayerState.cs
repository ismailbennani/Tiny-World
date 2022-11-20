using System;
using Character.Player;
using UnityEngine;

namespace State
{
    [Serializable]
    public class PlayerState
    {
        public PlayerConfig config;
        public Vector3 position;

        [Header("Computed state")]
        public Vector2Int playerTile;

        public void Update()
        {
            GameState state = GameStateManager.Current;
            if (state.map == null)
            {
                return;
            }
            
            playerTile = state.map.GetTileAt(position);
        }
    }
}
