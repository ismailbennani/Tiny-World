using Map.Tile;
using State;
using UnityEditor;
using UnityEngine;
using Utils;

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
                DrawMapState(gameState.map);
            }

            if (showPlayer)
            {
                DrawPlayerState(gameState.map, gameState.player);
            }
        }

        private void DrawMapState(MapState state)
        {
            if (state?.config == null)
            {
                return;
            }

            for (int x = 0; x < state.config.mapSize.x; x++)
            for (int y = 0; y < state.config.mapSize.y; y++)
            {
                Vector3 position = GetTilePosition(state, new Vector2Int(x, y));

                int index = MyMath.GetIndex(x, y, state.config.mapSize);
                Gizmos.color = GetColorFromTileType(state.tiles[index].type);
                Gizmos.DrawWireCube(position, new Vector3(state.config.tileSize.x, 0, state.config.tileSize.y));

                Handles.Label(position, index.ToString());
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

            Vector3 playerTilePosition = GetTilePosition(map, state.playerTile);
            Gizmos.DrawWireCube(playerTilePosition, new Vector3(map.config.tileSize.x * 0.9f, 0, map.config.tileSize.y * 0.9f));
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

        private static Vector3 GetTilePosition(MapState map, Vector2Int tile)
        {
            return map.mapOrigin + new Vector3(tile.x * (map.config.tileSize.x + map.config.gap.x), 0, tile.y * (map.config.tileSize.y + map.config.gap.y));
        }

        #endif
    }
}
