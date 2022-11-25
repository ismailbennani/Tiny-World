using System;
using System.Collections.Generic;
using Map;
using Map.Tile;
using Resource;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character
{
    public class GatherResourceController : MonoBehaviour
    {
        [Header("Mine")]
        public bool allowMine;
        public AudioClip[] mineAudioClips;
        public float mineAudioClipVolume;
        
        [Header("Chop")]
        public bool allowChop;
        public AudioClip[] chopAudioClips;
        public float chopAudioClipVolume;
        
        private int _animIDMine;
        private int _animIDChop;

        private Animator _animator;
        private ThirdPersonController _thirdPersonController;

        private ResourceType _currentResource;

        void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _animIDMine = Animator.StringToHash("Mine");
            _animIDChop = Animator.StringToHash("Chop");

            TryGetComponent(out _thirdPersonController);
            if (_thirdPersonController)
            {
                _thirdPersonController.onMoveStart.AddListener(CancelGather);
            }
        }

        public void Mine()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.player.playerTile);
            if (tile.config.resource != ResourceType.Stone || !tile.HasResource)
            {
                return;
            }
            
            _currentResource = ResourceType.Stone;
            _animator.SetBool(_animIDMine, true);
            
        }
        
        public void Chop()
        {
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }
            
            TileState tile = state.map.GetTile(state.player.playerTile);
            if (tile.config.resource != ResourceType.Wood || !tile.HasResource)
            {
                return;
            }
            
            _currentResource = ResourceType.Wood;
            _animator.SetBool(_animIDMine, true);
        }

        public void CancelGather()
        {
            _currentResource = ResourceType.None;
            _animator.SetBool(_animIDMine, false);
            _animator.SetBool(_animIDChop, false);
        }

        void OnGather()
        {
            if (_currentResource == ResourceType.None)
            {
                return;
            }
            
            GameState state = GameStateManager.Current;
            if (state == null)
            {
                return;
            }

            TileState tile = state.map.GetTile(state.player.playerTile);
            
            int actualConsumption = tile.ConsumeResource(1);
            state.resource.Produce(_currentResource, actualConsumption);

            switch (_currentResource)
            {
                case ResourceType.None:
                    throw new InvalidOperationException("Cannot happen");
                case ResourceType.Wood:
                    PlayClip(chopAudioClips, chopAudioClipVolume);
                    break;
                case ResourceType.Stone:
                    PlayClip(mineAudioClips, mineAudioClipVolume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!tile.HasResource)
            {
                CancelGather();
            }
        }

        private void PlayClip(IReadOnlyList<AudioClip> clips, float volume)
        {
            if (clips.Count <= 0)
            {
                return;
            }

            int index = Random.Range(0, clips.Count);
            AudioSource.PlayClipAtPoint(clips[index], transform.position, volume);
        }
    }
}
