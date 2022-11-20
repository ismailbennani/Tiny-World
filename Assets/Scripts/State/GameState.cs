using System;
using Unity.VisualScripting;

namespace State
{
    [Serializable]
    public class GameState
    {
        public MapState map = new();
        public PlayerState player = new();
        
        #region Static

        public static GameState Current => _current;
        
        [Serialize]
        private static GameState _current;

        public static GameState Initialize()
        {
            _current ??= new GameState();

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
}
