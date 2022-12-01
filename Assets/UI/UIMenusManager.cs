using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Input;
using UnityEngine;

namespace UI
{
    public class UIMenusManager : MonoBehaviour
    {
        private const float OpenCloseDelay = 0.5f;
        
        public static UIMenusManager Instance { get; private set; }

        public UIWindow mainMenu;
        public UIWindow inventory;

        public bool Visible => windowStack.Count > 0;
        
        [SerializeField]
        private List<UIWindow> windowStack = new();
        private static float _lastOpenCloseTime;

        void Start()
        {
            CloseAll();
        }

        void OnEnable()
        {
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
                Open(mainMenu);
            }
        }

        public void OpenInventory()
        {
            Open(inventory);
        }

        public void Open(UIWindow window)
        {
            if (!CanOpenClose())
            {
                return;
            }
            
            if (windowStack.Count == 0)
            {
                OnMenuOpen();
            }

            if (windowStack.Count > 0)
            {
                if (windowStack.Last() == window)
                {
                    return;
                }

                Show(windowStack.Last(), false);
            }

            Show(window, true);
            windowStack.Add(window);
        }

        public void Close(UIWindow uiWindow)
        {
            if (!CanOpenClose())
            {
                return;
            }
            
            if (windowStack.Count == 0 || windowStack.Last() != uiWindow)
            {
                return;
            }

            Show(uiWindow, false);
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
            if (!window)
            {
                return;
            }

            if (show)
            {
                window.Show();
                OnShow(window);
            }
            else
            {
                window.Hide();
            }
        }

        private void OnShow(UIWindow window)
        {
            StartCoroutine(DelayedFocus(window));
        }

        private void CloseAll()
        {
            ResetState();
            SwitchToPlayer();
        }

        private void HideAll()
        {
            Show(mainMenu, false);
            Show(inventory, false);
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

        private IEnumerator DelayedFocus(UIWindow window)
        {
            yield return null;

            window.Focus();
        }

        private bool CanOpenClose()
        {
            float now = Time.time;
                    
            if (_lastOpenCloseTime > now - OpenCloseDelay)
            {
                return false;
            }

            _lastOpenCloseTime = now;
            return true;
        }
    }
}
