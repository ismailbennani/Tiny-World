using Input;
using TMPro;
using UnityEngine;

namespace UI.Controls
{
    public class UIControlLine: MonoBehaviour
    {
        public TextMeshProUGUI text;

        public void SetFromCallback(GameInputCallback callback)
        {
            if (text)
            {
                text.SetText(callback.ActionName);
            }
        }
    }
}
