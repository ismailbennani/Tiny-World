using System.Collections.Generic;
using Map.Tile;
using UnityEngine;
using Utils;

namespace Map.Chunk
{
    public class MapChunk: MonoBehaviour
    {
        public List<MapTile> tiles; 
        public ChunkState state;

        [SerializeField]
        private MapTile baseTile;

        private bool _lastSetWasUrgent;

        public void Set(MapState map, Vector2Int chunk, bool urgent)
        {
            if (state?.position == chunk && (_lastSetWasUrgent || !urgent))
            {
                return;
            }
            
            state = map.GetChunk(chunk);
            baseTile = map.runtimeConfig.baseTile;

            tiles ??= new List<MapTile>();

            int nTiles = state.size.x * state.size.y;
            
            for (int i = tiles.Count; i < nTiles; i++)
            {
                MapTile newTile = Instantiate(baseTile, transform);
                tiles.Add(newTile);
            }

            for (int index = 0; index < nTiles; index++)
            {
                MapTile tile = tiles[index];
                tile.gameObject.SetActive(true);
                
                (int x, int y) = MyMath.GetCoords(index, state.size);
                TileState tileConfig = state.GetTile(x, y);
                
                tile.SetConfig(tileConfig, urgent);

                Vector3 position = map.GetTileCenterPosition(x + state.gridPosition.x, y + state.gridPosition.y);
                tile.transform.position = position;
            }

            for (int index = nTiles; index < tiles.Count; index++)
            {
                tiles[index].gameObject.SetActive(false);
            }
            
            _lastSetWasUrgent = urgent;
        }
    }
}
