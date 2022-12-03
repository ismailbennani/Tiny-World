using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Input;
using UnityEngine;

namespace UI
{
    public class UIMenusManager : MonoBehaviour
    {
        private const float MenuActionDelay = 0.5f;

        public static UIMenusManager Instance { get; private set; }

        public UIWindow mainMenu;
        public UIWindow inventory;

        [Space(10)]
        public UIDropdown dropdown;

        [Header("Runtime")]
        [SerializeField]
        private List<UIWindow> windowStack = new();

        private static float _lastMenuAction;

        void Start()
        {
            CloseAll();
        }

        void OnEnable()
        {
            HideAll();
            StartCoroutine(
                Delay(
                    () =>
                    {
                        if (windowStack.Any())
                        {
                            Show(windowStack.Last(), true);
                            SwitchToUi();
                        }
                        else
                        {
                            SwitchToPlayer();
                        }
                    }
                )
            );

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
            if (!CanPerformAction())
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
            if (!CanPerformAction())
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

        public bool OpenDropdown(IEnumerable<UIDropdownChoice> choices, Vector2 position)
        {
            if (!CanPerformAction())
            {
                return false;
            }

            if (windowStack.Count > 0)
            {
                windowStack.Last().FocusOut();
            }
            
            dropdown.Show(choices.Select(c => new UIDropdownChoice(c.Label, CreateDropdownCallback(c))).ToList(), position);
            StartCoroutine(Delay(dropdown.Focus));

            return true;
        }

        public bool CloseDropdown()
        {
            if (!CanPerformAction())
            {
                return false;
            }
            
            dropdown.Hide();

            if (windowStack.Count > 0)
            {
                StartCoroutine(Delay(windowStack.Last().FocusIn));
            }

            return true;
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
                StartCoroutine(Delay(window.FocusIn));
            }
            else
            {
                window.Hide();
            }
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

        private Action CreateDropdownCallback(UIDropdownChoice c)
        {
            return () =>
            {
                if (!CloseDropdown())
                {
                    return;
                }

                c.Callback?.Invoke();
            };
        }

        private bool CanPerformAction()
        {
            float now = Time.time;

            if (_lastMenuAction > now - MenuActionDelay)
            {
                return false;
            }

            _lastMenuAction = now;
            return true;
        }

        private static IEnumerator Delay(Action action)
        {
            yield return null;

            action?.Invoke();
        }
    }
}
