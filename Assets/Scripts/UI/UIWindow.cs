using UI.Theme;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        public GameObject root;
        public UITheme defaultTheme;
        public UITheme currentTheme;

        private void Start()
        {
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

            currentTheme = theme;
        }

        public void SaveTheme(UITheme theme)
        {
            if (!theme)
            {
                return;
            }

            SaveThemeInternal(theme);
        }

        public void Open()
        {
            root.SetActive(true);

            OnOpen();
        }

        public void Close()
        {
            root.SetActive(false);

            OnClose();
        }

        public void Toggle()
        {
            if (root.activeInHierarchy)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        protected abstract void OnOpen();
        protected abstract void OnClose();
        protected abstract void SetThemeInternal(UITheme theme);
        protected abstract void SaveThemeInternal(UITheme theme);
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
