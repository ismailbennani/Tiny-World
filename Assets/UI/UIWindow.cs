using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class UIWindow: MonoBehaviour
    {
        public UIDocument root;

        [NonSerialized]
        private bool _registered;
        
        void Start()
        {
            Hide();
        }

        void OnEnable()
        {
            if (!root)
            {
                throw new InvalidOperationException("Root UIDocument is null");
            }
            
            RegisterCallbacks();
        }

        protected abstract void RegisterAdditionalCallbacks();

        protected abstract void OnFocus();

        public void Show()
        {
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

        private void RegisterCallbacks()
        {
            UIManager uiManager = UIManager.Instance;
            if (_registered || !uiManager)
            {
                return;
            }
            
            root.rootVisualElement.RegisterCallback<NavigationCancelEvent>(
                evt =>
                {
                    uiManager.CloseCurrent();
                    evt.StopPropagation();
                }
            );

            root.rootVisualElement.Q<Button>("CloseButton").clicked += uiManager.CloseCurrent;
            
            RegisterAdditionalCallbacks();

            _registered = true;
        }
    }
}
