using Character.Player;
using Map;
using Map.Tile;
using UnityEditor;
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

            for (int index = 0; index < state.tiles.Length; index++)
            {
                Vector3 position = state.GetTileCenterPosition(index);
                
                Gizmos.color = GetColorFromTileType(state.tiles[index].config.type);
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

            Vector3 playerTilePosition = map.GetTileCenterPosition(state.playerTile);
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

        #endif
    }
}
