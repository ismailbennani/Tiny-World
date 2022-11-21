using Character.Player;
using Map;
using Map.Chunk;
using Map.Tile;
using UnityEngine;

namespace EditorUtils
{
    public class GameStateVisualizer : MonoBehaviour
    {
        #if UNITY_EDITOR
        
        [Header("Gizmos")]
        public bool showMap;
        public bool showPlayer;

        private void OnDrawGizmosSelected()
        {
            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            if (showMap)
            {
                DrawMapState(gameState.map, gameState.player);
            }

            if (showPlayer)
            {
                DrawPlayerState(gameState.map, gameState.player);
            }
        }

        private void DrawMapState(MapState map, PlayerState player)
        {
            if (map?.initialConfig == null)
            {
                return;
            }

            Vector2Int chunkPosition = player.playerChunk;
            ChunkState chunk = map.GetChunk(chunkPosition);

            Vector3 center = map.GetTileCenterPosition(chunk.gridPosition + chunk.size / 2);
            Vector2 size = chunk.size * map.runtimeConfig.tileSize;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, new Vector3(size.x, 0, size.y));
        }

        private void DrawPlayerState(MapState map, PlayerState state)
        {
            if (state == null)
            {
                return;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(state.position, 0.2f);

            Vector3 playerTilePosition = map.GetTileCenterPosition(state.playerTile);
            Gizmos.DrawWireCube(playerTilePosition, new Vector3(map.runtimeConfig.tileSize.x * 0.9f, 0, map.runtimeConfig.tileSize.y * 0.9f));
        }

        private static Color GetColorFromTileType(TileType type)
        {
            return type switch
            {
                TileType.Grass => Color.green,
                TileType.Gravel => Color.gray,
                _ => Color.clear,
            };
        }

        #endif
    }
}
