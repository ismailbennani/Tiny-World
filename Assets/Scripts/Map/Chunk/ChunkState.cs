using System;
using System.Collections.Generic;
using Items;
using Map.Tile;
using UnityEngine;
using Utils;

namespace Map.Chunk
{
    [Serializable]
    public class ChunkState
    {
        /// <summary>
        /// Chunk position amongst other chunks
        /// </summary>
        public Vector2Int position;

        /// <summary>
        /// Chunk position (bottom-left corner) in the grid
        /// </summary>
        public Vector2Int gridPosition;

        /// <summary>
        /// Chunk size in tiles
        /// </summary>
        /// <remarks>
        /// tiles for this chunk range from <c>gridPosition</c> to <c/>gridPosition + size</c> 
        /// </remarks>
        public Vector2Int size;
        
        public TileState[] tiles;
        public List<ItemState> items;

        public TileState GetTile(int x, int y)
        {
            if (x < 0 || x >= size.x || y < 0 || y >= size.y)
            {
                throw new InvalidOperationException($"Coordinates out of bounds: ({x}, {y}) outside of {size}");
            }

            int index = MyMath.GetIndex(x, y, size);
            return tiles[index];
        }
    }
}
