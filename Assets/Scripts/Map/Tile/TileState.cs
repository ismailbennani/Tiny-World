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
    }
}
