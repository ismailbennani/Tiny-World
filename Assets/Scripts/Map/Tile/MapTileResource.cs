using System.Collections;
using UnityEngine;

namespace Map.Tile
{
    public class MapTileResource: MonoBehaviour
    {
        private TileState _state;
        
        private Animator _animator;
        private int _animatorGatherId;
        
        void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _animatorGatherId = Animator.StringToHash("Consume");
            
            if (_state != null)
            {
                StartCoroutine(UpdateWhenGameStateReady());
            }
        }

        public void SetTile(TileState state)
        {
            _state = state;
            StartCoroutine(UpdateWhenGameStateReady());
        }

        public void OnGather()
        {
            if (_animator)
            {
                _animator.ResetTrigger(_animatorGatherId);
                _animator.SetTrigger(_animatorGatherId);
            }
        }

        private IEnumerator UpdateWhenGameStateReady()
        {
            while (!GameStateManager.Ready)
            {
                yield return null;
            }

            MapState map = GameStateManager.Current.map;
            
            SetSize(map.runtimeConfig.resourceScale);
        }

        private void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }
    }
}
