using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.Animation
{
    public class AnimateChildrenAnimation: MonoBehaviour, IAnimation
    {
        public GameObject[] children = { };

        private IAnimation[] _childrenAnimations = { };

        private void OnEnable()
        {
            _childrenAnimations = children.SelectMany(c => c.GetComponents<IAnimation>())
                .Where(a => a != null).ToArray();
        }

        public IEnumerator Animate()
        {
            List<IEnumerator> enumerators = _childrenAnimations.Select(c => c.Animate()).ToList();

            while (enumerators.Any())
            {
                enumerators.RemoveAll(e => !e.MoveNext());

                yield return null;
            }
        }
    }
}
