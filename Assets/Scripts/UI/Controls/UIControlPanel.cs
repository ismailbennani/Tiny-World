using System;
using System.Linq;
using Input;
using UnityEngine;

namespace UI.Controls
{
    public class UIControlPanel : MonoBehaviour
    {
        public GameObject defaultPanel;
        public UIControlPanelForInputScheme[] panelsByScheme;

        void Update()
        {
            HideAll();
            
            GameInputAdapter inputAdapter = GameInputAdapter.Instance;
            if (!inputAdapter || !inputAdapter.playerInput || !ShowPanelForScheme(inputAdapter.playerInput.currentControlScheme))
            {
                ShowDefaultPanel();
            }
        }

        private void ShowDefaultPanel()
        {
            if (defaultPanel)
            {
                defaultPanel.gameObject.SetActive(true);
            }
        }

        private bool ShowPanelForScheme(string scheme)
        {
            GameObject obj = panelsByScheme.FirstOrDefault(p => p.scheme == scheme)?.obj;
            if (obj)
            {
                obj.SetActive(true);
                return true;
            }

            return false;
        }

        private void HideAll()
        {
            if (defaultPanel)
            {
                defaultPanel.gameObject.SetActive(false);
            }

            foreach (UIControlPanelForInputScheme panel in panelsByScheme)
            {
                if (panel.obj)
                {
                    panel.obj.SetActive(false);
                }
            }
        }
    }

    [Serializable]
    public class UIControlPanelForInputScheme
    {
        public string scheme;
        public GameObject obj;
    }
}
