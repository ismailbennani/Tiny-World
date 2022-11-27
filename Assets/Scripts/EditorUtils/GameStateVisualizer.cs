using Character.Player;
using Map;
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
            Rect rect = map.GetChunkRect(chunkPosition);

            Vector3 center = new(rect.center.x, 1, rect.center.y);
            Vector3 size = new(rect.size.x, 0, rect.size.y);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(center, size);
            
            for (int x = 0; x < map.initialConfig.chunkSize.x; x++)
            for (int y = 0; y < map.initialConfig.chunkSize.y; y++)
            {
                Rect tileRect = map.GetTileRect(chunkPosition * map.initialConfig.chunkSize + new Vector2Int(x, y));
                
                Vector3 tileCenter = new(tileRect.center.x, 1, tileRect.center.y);
                Vector3 tileSize = new(tileRect.size.x, 0, tileRect.size.y);
                
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(tileCenter, tileSize);
            }
        }

        private void DrawPlayerState(MapState map, PlayerState state)
        {
            if (state == null)
            {
                return;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(state.position, 0.2f);

            Rect tileRect = map.GetTileRect(state.playerTile);
            
            Vector3 tileCenter = new(tileRect.center.x, 1, tileRect.center.y);
            Vector3 tileSize = new(tileRect.size.x, 0, tileRect.size.y);
                
            Gizmos.DrawWireCube(tileCenter, tileSize);
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
