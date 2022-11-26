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
        public GatheredResourceParams mine;
        public GatheredResourceParams chop;

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

            PlayParticles(_currentResource);
            PlayClip(_currentResource);

            TriggerResourceAnimation(tile);

            if (!tile.HasResource)
            {
                CancelGather();
            }
        }

        private void PlayParticles(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.None:
                    throw new InvalidOperationException("Cannot happen");
                case ResourceType.Wood:
                    PlayParticles(chop.particles, new Vector3(0, 0.5f, 0.2f));
                    break;
                case ResourceType.Stone:
                    PlayParticles(mine.particles, new Vector3(0, 0, 0.2f));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayParticles(IReadOnlyList<GameObject> particles, Vector3 offset)
        {
            if (particles.Count <= 0)
            {
                return;
            }

            int index = Random.Range(0, particles.Count);
            Instantiate(particles[index], offset, Quaternion.identity, transform);
        }

        private void PlayClip(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.None:
                    throw new InvalidOperationException("Cannot happen");
                case ResourceType.Wood:
                    PlayClip(chop.audioClips, chop.audioVolume);
                    break;
                case ResourceType.Stone:
                    PlayClip(mine.audioClips, mine.audioVolume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        private void TriggerResourceAnimation(TileState tile)
        {
            GameMap map = GameMap.Instance;
            if (!map)
            {
                return;
            }

            MapTile mapTile = map.GetTile(tile);
            if (!mapTile)
            {
                return;
            }
            
            mapTile.TriggerResourceAnimation();
        }
    }

    [Serializable]
    public class GatheredResourceParams
    {
        public bool allow;

        [Header("Particles")]
        public GameObject[] particles;
        
        [Header("Audio")]
        public AudioClip[] audioClips;
        public float audioVolume;

    }
}
