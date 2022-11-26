using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Input
{
    public class GameInputCallbackManager : MonoBehaviour
    {
        public static GameInputCallbackManager Instance { get; private set; }

        public UnityEvent<GameInputType, GameInputCallback> onCallbackChange = new();

        private List<InputComponentCallback> _callbacks;

        void OnEnable()
        {
            Instance = this;

            _callbacks = new List<InputComponentCallback>();
        }

        public void Register(GameInputType inputType, Component component, GameInputCallback callback)
        {
            RunAndNotifyOnCallbackChange(inputType, () =>
            {
                UnregisterInternal(inputType, component);
                _callbacks.Add(new InputComponentCallback(inputType, component, callback));
            });
        }

        public void Unregister(GameInputType inputType, Component component)
        {
            RunAndNotifyOnCallbackChange(inputType, () => UnregisterInternal(inputType, component));
        }

        public void UnregisterAll(Component component)
        {
            foreach (InputComponentCallback callback in _callbacks.Where(c => c.Component == component).ToArray())
            {
                Unregister(callback.InputType, callback.Component);
            }
        }

        public GameInputCallback GetCallback(GameInputType inputType)
        {
            return _callbacks?.Where(c => c.InputType == inputType).OrderBy(c => c.Callback.Priority).FirstOrDefault()?.Callback;
        }

        // called through SendMessage

        public void OnInput(GameInputType inputType)
        {
            GameInputCallback callback = GetCallback(inputType);
            callback?.Callback?.Invoke();
        }

        private void RunAndNotifyOnCallbackChange(GameInputType inputType, Action func)
        {
            GameInputCallback oldCallback = GetCallback(inputType);

            func();

            GameInputCallback newCallback = GetCallback(inputType);
            if (newCallback?.ActionName != oldCallback?.ActionName)
            {
                onCallbackChange?.Invoke(inputType, newCallback);
            }
        }

        private void UnregisterInternal(GameInputType inputType, Component component)
        {
            _callbacks.RemoveAll(c => c.InputType == inputType && c.Component == component);
        }

        private class InputComponentCallback
        {
            public readonly GameInputType InputType;
            public readonly Component Component;
            public readonly GameInputCallback Callback;

            public InputComponentCallback(GameInputType inputType, Component component, GameInputCallback callback)
            {
                InputType = inputType;
                Component = component;
                Callback = callback;
            }
        }
    }
}
