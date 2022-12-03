using UnityEngine;

namespace Map.Tile.Animation
{
    public class MapTileResourceAnimatorAnimation: MonoBehaviour, IMapTileResourceAnimation
    {
        private Animator _animator;
        private int _animatorGatherId;
        
        void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _animatorGatherId = Animator.StringToHash("Consume");
        }
        
        public void OnLoot()
        {
            if (_animator)
            {
                _animator.ResetTrigger(_animatorGatherId);
                _animator.SetTrigger(_animatorGatherId);
            }
        }
    }
}
