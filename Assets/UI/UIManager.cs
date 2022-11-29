using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public UIDocument root;

        public bool Visible => windowStack.Count > 0;

        private VisualElement _mainMenu;
        private VisualElement _inventory;

        [SerializeField]
        private List<UIWindow> windowStack = new();

        void Start()
        {
            GetElements();
            CloseAll();
        }

        void OnEnable()
        {
            GetElements();

            HideAll();
            if (windowStack.Any())
            {
                Show(windowStack.Last(), true);
                SwitchToUi();
            }
            else
            {
                SwitchToPlayer();
            }

            Instance = this;
        }

        public void Toggle()
        {
            if (windowStack.Count > 0)
            {
                CloseAll();
            }
            else
            {
                Open(UIWindow.MainMenu);
            }
        }

        public void Open(UIWindow window)
        {
            if (windowStack.Count == 0)
            {
                OnMenuOpen();
            }

            if (windowStack.Count > 0)
            {
                Show(windowStack.Last(), false);
            }

            Show(window, true);
            windowStack.Add(window);
        }

        public void CloseCurrent()
        {
            if (windowStack.Count == 0)
            {
                return;
            }

            Show(windowStack.Last(), false);
            windowStack.RemoveAt(windowStack.Count - 1);

            if (windowStack.Count > 0)
            {
                Show(windowStack.Last(), true);
            }
            else if (windowStack.Count == 0)
            {
                OnMenuClose();
            }
        }

        private void Show(UIWindow window, bool show)
        {
            VisualElement windowElement = GetWindowElement(window);

            if (windowElement != null)
            {
                windowElement.visible = show;
                windowElement.SetEnabled(show);
            }
            
            OnShow(window);
        }

        private VisualElement GetWindowElement(UIWindow window)
        {
            VisualElement windowElement = window switch
            {
                UIWindow.MainMenu => _mainMenu,
                UIWindow.Inventory => _inventory,
                _ => throw new ArgumentOutOfRangeException(nameof(window), window, null)
            };

            return windowElement;
        }

        private void OnShow(UIWindow window)
        {
            switch (window)
            {
                case UIWindow.MainMenu:
                    StartCoroutine(DelayedFocus(() => root.rootVisualElement.Q<Button>("InventoryMenuButton")));
                    
                    root.rootVisualElement.Q<Button>("InventoryMenuButton").Focus();
                    break;
                case UIWindow.Inventory:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(window), window, null);
            }
        }

        private void CloseAll()
        {
            ResetState();
            SwitchToPlayer();
        }

        private void HideAll()
        {
            foreach (UIWindow window in Enum.GetValues(typeof(UIWindow)).OfType<UIWindow>())
            {
                Show(window, false);
            }
        }

        private void ResetState()
        {
            HideAll();
            windowStack.Clear();
        }

        private void OnMenuOpen()
        {
            ResetState();
            SwitchToUi();
        }

        private void OnMenuClose()
        {
            SwitchToPlayer();
        }

        private void SwitchToUi()
        {
            GameInputAdapter gameInputAdapter = GameInputAdapter.Instance;
            if (!gameInputAdapter)
            {
                return;
            }

            gameInputAdapter.SwitchToUi();
        }

        private void SwitchToPlayer()
        {
            GameInputAdapter gameInputAdapter = GameInputAdapter.Instance;
            if (!gameInputAdapter)
            {
                return;
            }

            gameInputAdapter.SwitchToPlayer();
        }

        private void GetElements()
        {
            if (!root)
            {
                root = GetComponent<UIDocument>();
            }

            _mainMenu = root.rootVisualElement.Q<VisualElement>("MainMenu");
            _inventory = root.rootVisualElement.Q<VisualElement>("Inventory");
        }

        private IEnumerator DelayedFocus(Func<VisualElement> getter)
        {
            yield return null;

            VisualElement element = getter();
            element?.Focus();
        }
    }

    public enum UIWindow
    {
        MainMenu,
        Inventory,
    }
}
