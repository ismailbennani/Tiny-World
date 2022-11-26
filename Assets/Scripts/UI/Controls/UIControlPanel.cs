using System;
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
            GameInputAdapter inputAdapter = GameInputAdapter.Instance;
            if (!inputAdapter || !inputAdapter.playerInput || !ShowPanelForScheme(inputAdapter.playerInput.currentControlScheme))
            {
                ShowDefaultPanel(true);
            }
            else
            {
                ShowDefaultPanel(false);
            }
        }

        private void ShowDefaultPanel(bool show)
        {
            if (defaultPanel)
            {
                defaultPanel.gameObject.SetActive(show);
            }
        }

        private bool ShowPanelForScheme(string scheme)
        {
            bool atLeastOne = false;
            
            foreach (UIControlPanelForInputScheme panel in panelsByScheme)
            {
                if (!panel.obj)
                {
                    continue;
                }

                bool show = string.Equals(panel.scheme, scheme, StringComparison.InvariantCultureIgnoreCase);
                panel.obj.SetActive(show);

                atLeastOne = atLeastOne || show;
            }

            return atLeastOne;
        }
    }

    [Serializable]
    public class UIControlPanelForInputScheme
    {
        public string scheme;
        public GameObject obj;
    }
}
