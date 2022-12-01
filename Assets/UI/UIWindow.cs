using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class UIWindow: MonoBehaviour
    {
        public UIDocument root;

        protected Button CloseButton;
        
        [NonSerialized]
        private bool _registering;
        [NonSerialized]
        private bool _registered;

        void Start()
        {
            Hide();
        }

        protected virtual void OnEnable()
        {
            if (!root)
            {
                throw new InvalidOperationException("Root UIDocument is null");
            }

            root.rootVisualElement.style.position = new StyleEnum<Position>(Position.Absolute);
            root.rootVisualElement.style.top = new StyleLength(0f);
            root.rootVisualElement.style.right = new StyleLength(0f);
            root.rootVisualElement.style.bottom = new StyleLength(0f);
            root.rootVisualElement.style.left = new StyleLength(0f);

            CloseButton = root.rootVisualElement.Q<Button>("CloseButton");

            StartCoroutine(RegisterCallbacksWhenReady());
        }

        protected abstract void RegisterAdditionalCallbacks();
        protected abstract void Load();
        protected abstract void OnFocus();

        public void Show()
        {
            Load();
            root.rootVisualElement.visible = true;
        }

        public void Hide()
        {
            root.rootVisualElement.visible = false;
        }

        public void Focus()
        {
            OnFocus();
        }

        private IEnumerator RegisterCallbacksWhenReady()
        {
            if (_registering || _registered)
            {
                yield break;
            }

            _registering = true;
            
            while (!UIMenusManager.Instance)
            {
                yield return null;
            }

            root.rootVisualElement.RegisterCallback<NavigationCancelEvent>(
                evt =>
                {
                    UIMenusManager.Instance.Close(this);
                    evt.StopImmediatePropagation();
                }
            );

            if (CloseButton != null)
            {
                CloseButton.clicked += () => UIMenusManager.Instance.Close(this);
            }

            RegisterAdditionalCallbacks();

            _registering = false;
            _registered = true;
        }
    }
}
