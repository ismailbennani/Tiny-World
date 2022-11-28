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
                OpenMainMenu();
            }
            else
            {
                CloseCurrent();
            }
        }
        

        public void OpenMainMenu()
        {
            CloseAll();
            Open(mainMenu);
        }

        public void OpenInventory()
        {
            Open(inventory);
        }

        public void CloseCurrent()
        {
            if (_menuStack.Count >= 1)
            {
                _menuStack[^1].Close();
                _menuStack.RemoveAt(_menuStack.Count - 1);
            }

            if (_menuStack.Count >= 1)
            {
                _menuStack[^1].Open();
            }
            
            if (_menuStack.Count == 0)
            {
                OnMenuClose();
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

        private void Open(UIWindow window)
        {
            if (_menuStack.Count == 0)
            {
                OnMenuOpen();
            }
            
            if (_menuStack.Count > 1)
            {
                _menuStack[^1].Close();
            }

            window.Open();
            _menuStack.Add(window);
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
