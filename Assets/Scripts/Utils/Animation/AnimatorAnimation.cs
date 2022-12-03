using System.Collections;
using UnityEngine;

namespace Utils.Animation
{
    public class AnimatorAnimation: MonoBehaviour, IAnimation
    {
        public Animator animator;
        public string animationName;
        
        private int _animatorGatherId;
        
        void OnEnable()
        {
            animator = GetComponent<Animator>();
            _animatorGatherId = Animator.StringToHash(animationName);
        }
        
        public IEnumerator Animate()
        {
            if (!animator)
            {
                yield break;
            }
            
            animator.ResetTrigger(_animatorGatherId);
            animator.SetTrigger(_animatorGatherId);

            while (animator.GetAnimatorTransitionInfo(0).duration > 0)
            {
                yield return null;
            }
        }
    }
}
