using System;
using Input;
using UnityEngine;

namespace UI.Controls
{
    public class UIDynamicControls : MonoBehaviour
    {
        public UIControlLineForInputType[] imageForInputTypes;

        void Update()
        {
            GameInputCallbackManager gameInputCallbackManager = GameInputCallbackManager.Instance;
            if (gameInputCallbackManager)
            {
                foreach (UIControlLineForInputType line in imageForInputTypes)
                {
                    GameInputCallback callback = gameInputCallbackManager.GetCallback(line.inputType);

                    if (callback != null)
                    {
                        line.obj.gameObject.SetActive(true);
                        line.obj.SetFromCallback(callback);
                    }
                    else
                    {
                        line.obj.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (UIControlLineForInputType line in imageForInputTypes)
                {
                    line.obj.gameObject.SetActive(false);
                }
            }
        }
    }

    [Serializable]
    public class UIControlLineForInputType
    {
        public GameInputType inputType;
        public UIControlLine obj;
    }
}
