using System;
using UnityEngine;

namespace Map.Tile
{
    [Serializable]
    public class TileState
    {
        public TileConfig config;

        public Vector2Int position;
        
        [Range(0, 3)]
        public int rotation;

        public TileState(TileConfig config, Vector2Int position, int rotation)
        {
            this.config = config;
            this.position = position;
            this.rotation = Mathf.Clamp(rotation, 0, 3);
        }
    }
}
