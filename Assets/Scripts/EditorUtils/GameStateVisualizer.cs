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
        
        public bool map;
        public bool player;

        private void OnDrawGizmosSelected()
        {
            GameState state = GameState.Current;
            if (state == null)
            {
                return;
            }

            if (map)
            {
                DrawMapState(state.map);
            }

            if (player)
            {
                DrawPlayerState(state.player);
            }
        }

        private void DrawMapState(MapState state)
        {
            if (state == null || !state.config)
            {
                return;
            }

            Vector2 tileAndGap = state.config.tileSize + state.config.gap;
            Vector2 halfTileSize = state.config.tileSize / 2;
            Vector2 mapSize = (state.config.tileSize + state.config.gap) * state.config.mapSize - state.config.gap;
            Vector2 halfSize = mapSize / 2;

            for (int x = 0; x < state.config.mapSize.x; x++)
            for (int y = 0; y < state.config.mapSize.y; y++)
            {
                Vector3 position = new(x * tileAndGap.x - halfSize.x, 0, y * tileAndGap.y - halfSize.y);

                int index = MyMath.GetIndex(x, y, state.config.mapSize);
                Gizmos.color = GetColorFromTileType(state.tiles[index].type);
                Gizmos.DrawWireCube(position, new Vector3(state.config.tileSize.x, 0, state.config.tileSize.y));

                Handles.Label(position, index.ToString());
            }
        }

        private void DrawPlayerState(PlayerState state)
        {
            if (state == null)
            {
                return;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(state.position, 0.2f);
        }

        private Color GetColorFromTileType(TileType type)
        {
            return type switch
            {
                TileType.Grass => Color.green,
                TileType.Stone => Color.gray,
                _ => Color.clear,
            };
        }

        #endif
    }
}
