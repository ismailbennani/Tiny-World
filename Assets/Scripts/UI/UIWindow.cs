using System.Collections;
using TMPro;
using UI.Theme;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public abstract class UIWindow : MonoBehaviour, ICancelHandler
    {
        public GameObject root;
        public UITheme defaultTheme;
        public UITheme currentTheme;
        
        [Header("Main window")]
        public Image panel;
        public TextMeshProUGUI title;
        public UICloseButton closeButton;

        private void Start()
        {
            StartCoroutine(ApplyThemeWhenReady());
            Close();
        }

        public void SetTheme(UITheme theme)
        {
            if (!theme)
            {
                if (defaultTheme)
                {
                    SetTheme(defaultTheme);
                }

                return;
            }

            SetThemeInternal(theme);
            
            panel.sprite = theme.panel;
            title.font = theme.title.font;
            title.color = theme.title.color;
            
            closeButton.SetTheme(theme);

            currentTheme = theme;
        }

        public void SaveTheme(UITheme theme)
        {
            if (!theme)
            {
                return;
            }

            defaultTheme.panel = panel.sprite;
            defaultTheme.title.font = title.font;
            defaultTheme.title.color = title.color;
            
            closeButton.SaveTheme(theme);
            
            SaveThemeInternal(theme);
        }

        public void Open()
        {
            OnOpen();
            
            UnStash();

            UIMenuManager.Instance.Register(this);
        }

        public void Stash()
        {
            root.SetActive(false);
            closeButton.SetSelected(false);
        }

        public void UnStash()
        {
            root.SetActive(true);
            
            OnFocus();
        }

        public void Close()
        {
            Stash();

            OnClose();
            
            UIMenuManager.Instance.Unregister(this);
        }

        public void OnCancel(BaseEventData eventData)
        {
            UIMenuManager.Instance.CloseCurrent();
        }

        protected abstract void SetThemeInternal(UITheme theme);

        protected abstract void SaveThemeInternal(UITheme theme);

        protected virtual void OnOpen() { }
        protected virtual void OnFocus() { }

        protected virtual void OnClose() { }

        private IEnumerator ApplyThemeWhenReady()
        {
            while (!GameStateManager.Config)
            {
                yield return null;
            }

            SetTheme(GameStateManager.Config.theme);
        }
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(UIWindow), true)]
    public class UIWindowEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Open"))
            {
                UIWindow inventory = target as UIWindow;
                if (!inventory)
                {
                    return;
                }

                inventory.Open();
            }

            if (GUILayout.Button("Close"))
            {
                UIWindow inventory = target as UIWindow;
                if (!inventory)
                {
                    return;
                }

                inventory.Close();
            }

            if (GUILayout.Button("Apply theme"))
            {
                UIWindow inventory = target as UIWindow;
                if (!inventory)
                {
                    return;
                }

                if (!inventory.currentTheme)
                {
                    return;
                }

                inventory.SetTheme(inventory.currentTheme);
            }

            if (GUILayout.Button("Save theme to current"))
            {
                UIWindow inventory = target as UIWindow;
                if (!inventory)
                {
                    return;
                }

                UITheme currentTheme = inventory.currentTheme;
                if (!currentTheme)
                {
                    return;
                }

                inventory.SaveTheme(currentTheme);
            }
        }
    }

    #endif
}
