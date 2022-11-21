using System;
using UnityEngine;

namespace Character.Player
{
    [Serializable]
    public class PlayerState
    {
        public PlayerConfig config;
        public Vector3 position;

        [Header("Computed state")]
        public Vector2Int playerChunk;
        public Vector2Int playerTile;

        public void Update()
        {
            GameState state = GameStateManager.Current;
            if (state.map == null)
            {
                return;
            }
            
            playerChunk = state.map.GetChunkAt(position);
            playerTile = state.map.GetTileAt(position);
        }
    }
}
