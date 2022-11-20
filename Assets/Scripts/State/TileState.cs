using System;
using Map.Tile;
using UnityEngine;

namespace State
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
