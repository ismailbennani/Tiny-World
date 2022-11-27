using UnityEngine;

namespace Utils
{
    public class OutlinedGameObject: HighlightableGameObject
    {
        public Outline outline;

        void Start()
        {
            Unhighlight();
        }

        public override void Highlight()
        {
            if (!OutlineExists())
            {
                return;
            }

            outline.enabled = true;
        }

        public override void Unhighlight()
        {
            if (!OutlineExists())
            {
                return;
            }
            
            outline.enabled = false;
        }

        private bool OutlineExists()
        {
            if (!outline)
            {
                outline = GetComponent<Outline>();
            }

            if (!outline)
            {
                Debug.LogWarning("Missing outline");
                return false;
            }
            
            return true;
        }
    }
}
