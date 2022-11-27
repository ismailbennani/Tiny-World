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

        public void Register(GameInputType inputType, Component component, GameInputCallback callback, int channel = 0)
        {
            RunAndNotifyOnCallbackChange(
                inputType,
                () =>
                {
                    IEnumerable<InputComponentCallback> current = _callbacks.Where(
                        c => c.InputType == inputType && c.Component == component && c.Channel == channel
                    );
                    InputComponentCallback highest = current.OrderByDescending(c => c.Callback.Priority).FirstOrDefault();

                    if (highest != null && highest.Callback.Priority > callback.Priority)
                    {
                        return;
                    }

                    Unregister(inputType, component, channel);
                    _callbacks.Add(new InputComponentCallback(inputType, component, callback, channel));
                }
            );
        }

        public void Unregister(GameInputType inputType, Component component, int? channel = null)
        {
            RunAndNotifyOnCallbackChange(inputType, () => UnregisterInternal(inputType, component, channel));
        }

        public void UnregisterAll(Component component)
        {
            foreach (InputComponentCallback callback in _callbacks.Where(c => c.Component == component).ToArray())
            {
                Unregister(callback.InputType, callback.Component, null);
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

        private void UnregisterInternal(GameInputType inputType, Component component, int? channel)
        {
            _callbacks.RemoveAll(c => c.InputType == inputType && c.Component == component && (!channel.HasValue || c.Channel == channel.Value));
        }

        private class InputComponentCallback
        {
            public readonly GameInputType InputType;
            public readonly Component Component;
            public readonly GameInputCallback Callback;

            /// <summary>
            /// The same component can register multiple callbacks for the same input (with different priorities) in different channels
            /// </summary>
            public readonly int Channel;

            public InputComponentCallback(GameInputType inputType, Component component, GameInputCallback callback, int channel)
            {
                InputType = inputType;
                Component = component;
                Channel = channel;
                Callback = callback;
            }
        }
    }
}
