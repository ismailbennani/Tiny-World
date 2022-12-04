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
        protected abstract void OnOpen();
        protected abstract void OnFocusIn();
        protected abstract void OnFocusOut();
        protected abstract void OnClose();

        public void Show()
        {
            OnOpen();
            root.rootVisualElement.visible = true;
        }

        public void FocusIn()
        {
            OnFocusIn();
        }

        public void FocusOut()
        {
            OnFocusOut();
        }

        public void Hide()
        {
            OnClose();
            root.rootVisualElement.visible = false;
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
