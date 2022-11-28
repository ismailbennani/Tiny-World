using System.Collections.Generic;
using Input;
using UI.Inventory;
using UI.MainMenu;
using UnityEngine;

namespace UI
{
    public class UIMenuManager : MonoBehaviour
    {
        public static UIMenuManager Instance { get; private set; }

        public UIMainMenu mainMenu;
        public UIInventory inventory;

        [SerializeField]
        private List<UIWindow> _menuStack = new();

        void OnEnable()
        {
            Instance = this;

            if (_menuStack.Count > 0)
            {
                OnMenuOpen();
            }
            else
            {
                OnMenuClose();
            }
        }

        public void ToggleMenu()
        {
            if (_menuStack.Count == 0)
            {
                mainMenu.Open();
            }
            else
            {
                CloseCurrent();
            }
        }


        public void Register(UIWindow window)
        {
            if (_menuStack.Count == 0)
            {
                OnMenuOpen();
            }

            if (_menuStack.Count > 0)
            {
                _menuStack[^1].Stash();
            }

            _menuStack.Add(window);
        }

        public void Unregister(UIWindow window)
        {
            bool isCurrent = _menuStack.Count > 0 && window == _menuStack[^1];

            _menuStack.Remove(window);

            if (isCurrent && _menuStack.Count > 0)
            {
                _menuStack[^1].UnStash();
            }

            if (_menuStack.Count == 0)
            {
                OnMenuClose();
            }
        }

        public void CloseCurrent()
        {
            if (_menuStack.Count > 0)
            {
                _menuStack[^1].Close();
            }
        }

        public void CloseAll()
        {
            foreach (UIWindow window in _menuStack)
            {
                window.Close();
            }

            _menuStack.Clear();
            OnMenuClose();
        }

        private void OnMenuOpen()
        {
            GameInputAdapter gameInputAdapter = GameInputAdapter.Instance;
            if (!gameInputAdapter)
            {
                return;
            }

            gameInputAdapter.SwitchToUi();
        }

        private void OnMenuClose()
        {
            GameInputAdapter gameInputAdapter = GameInputAdapter.Instance;
            if (!gameInputAdapter)
            {
                return;
            }

            gameInputAdapter.SwitchToPlayer();
        }
    }
}
