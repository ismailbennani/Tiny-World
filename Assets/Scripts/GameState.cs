using System;
using Map;
using Map.Tile;
using Utils;

[Serializable]
public class GameState
{
    public MapConfig mapConfig;
    public TileConfig[] tiles;

    public TileConfig GetTileAt(int x, int y)
    {
        return tiles[MyMath.GetIndex(x, y, mapConfig.mapSize)];
    }

    #region Static

    public static GameState Current { get; private set; }

    public static GameState Initialize()
    {
        if (Current == null)
        {
            Current = new GameState();
        }

        return Current;
    }

    private static void AssertInitialized()
    {
        if (Current == null)
        {
            throw new InvalidOperationException("Game state not initialized yet");
        }
    }

    #endregion
}
