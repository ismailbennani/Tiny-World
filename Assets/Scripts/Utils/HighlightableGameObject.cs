using UnityEngine;

namespace Utils
{
    public abstract class HighlightableGameObject: MonoBehaviour
    {
        public abstract void Highlight();
        public abstract void Unhighlight();
    }
}
