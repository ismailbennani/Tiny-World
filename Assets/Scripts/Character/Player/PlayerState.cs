using System;
using Map;
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
            PlayerController controller = PlayerController.Instance;
            if (!controller)
            {
                return;
            }

            position = controller.transform.position;
            
            GameState state = GameStateManager.Current;
            if (!state || state.map == null)
            {
                return;
            }
            
            playerChunk = state.map.GetChunkPositionAt(position);
            playerTile = state.map.GetTilePositionAt(position);
        }
    }
}
