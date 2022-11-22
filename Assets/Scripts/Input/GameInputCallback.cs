using System;

namespace Input
{
    public class GameInputCallback
    {
        public readonly string ActionName;
        public readonly int Priority;
        public readonly Action Callback;

        public GameInputCallback(string actionName, Action callback, int priority = 0)
        {
            ActionName = actionName;
            Callback = callback;
            Priority = priority;
        }
    }
}
