using System;
using System.Collections;
using System.Linq;
using Character.Player;
using Input;
using Map;
using Map.Tile;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Character
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        public CharacterState state;
        public PlayerControllerInputSource playerControllerInputSource;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        [Header("Audio")]
        public AudioClipsForTileType[] landingByTileType;
        public AudioClip[] defaultLandingAudioClips;
        public AudioClipsForTileType[] footstepByTileType;
        public AudioClip[] defaultFootstepAudioClips;
        [Range(0, 1)]
        public float footstepAudioVolume = 0.5f;

        [Header("Camera")]
        public Transform cameraTarget;
        public float cameraAngle;
        public float zoomLevel;

        [Header("Events")]
        public UnityEvent onMoveStart = new();
        public UnityEvent onMoveEnd = new();

        // player
        private Vector2 _moveInput;
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _cameraRotationVelocity;

        // timeout delta time
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private CharacterController _controller;
        private GameObject _mainCamera;

        private Coroutine _registerSprintCoroutine;

        private bool _hasAnimator;
        private bool _moving;

        void Start()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            playerControllerInputSource = GetComponent<PlayerControllerInputSource>();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = state.config.jumpTimeout;
            _fallTimeoutDelta = state.config.fallTimeout;
            
            _registerSprintCoroutine = StartCoroutine(RegisterSprintCommand());
        }

        void OnEnable()
        {
            Debug.Log("PrintOnEnable: script was enabled");
            _registerSprintCoroutine = StartCoroutine(RegisterSprintCommand());
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            UpdateCamera();
        }

        private void LateUpdate()
        {
            if (state == null)
            {
                return;
            }

            state.position = transform.position;

            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return;
            }

            state.chunk = gameState.map.GetChunkPositionAt(state.position);
            state.tile = gameState.map.GetTilePositionAt(state.position);
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 position = transform.position;
            Vector3 spherePosition = new(position.x, position.y - state.config.groundedOffset, position.z);
            grounded = Physics.CheckSphere(spherePosition, state.config.groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, grounded);
            }
        }

        private void Move()
        {
            // don't change speed if we are not grounded because it would change speed mid-air
            if (grounded)
            {
                _moveInput = playerControllerInputSource.move;
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = state.sprint ? state.config.sprintSpeed : state.config.moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_moveInput == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            Vector3 velocity = _controller.velocity;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = playerControllerInputSource.analogMovement ? _moveInput.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * state.config.speedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * state.config.speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_moveInput != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, state.config.rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                if (!_moving)
                {
                    onMoveStart?.Invoke();
                }

                _moving = true;
            }
            else
            {
                if (_moving)
                {
                    onMoveEnd?.Invoke();
                }

                _moving = false;
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = state.config.fallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (playerControllerInputSource.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(state.config.jumpHeight * -2f * state.config.gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = state.config.jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                playerControllerInputSource.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += state.config.gravity * Time.deltaTime;
            }
        }

        private void UpdateCamera()
        {
            if (state.config is not PlayerConfig playerConfig)
            {
                return;
            }

            cameraAngle = Mathf.SmoothDampAngle(
                              cameraAngle,
                              cameraAngle + playerControllerInputSource.look * 10,
                              ref _cameraRotationVelocity,
                              playerConfig.cameraRotationSmoothTime
                          )
                          % 360;

            float targetZoom = playerControllerInputSource.zoom > 0.5
                ? 1
                : playerControllerInputSource.zoom < -0.5
                    ? 0
                    : zoomLevel;
            zoomLevel = Mathf.SmoothStep(zoomLevel, targetZoom, playerConfig.zoomSpeed);
        }

        private IEnumerator RegisterSprintCommand()
        {
            while (!GameInputCallbackManager.Instance || state == null)
            {
                yield return null;
            }

            GameInputCallbackManager gameInputCallbackManager = GameInputCallbackManager.Instance;

            GameInputCallback callback = new(state.sprint ? "Walk" : "Sprint", ToggleSprint);
            gameInputCallbackManager.Register(GameInputType.ToggleSprint, this, callback);

            _registerSprintCoroutine = null;
        }

        private void ToggleSprint()
        {
            state.sprint = !state.sprint;
            
            if (_registerSprintCoroutine == null)
            {
                _registerSprintCoroutine = StartCoroutine(RegisterSprintCommand());
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Vector3 position = transform.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - state.config.groundedOffset, position.z), state.config.groundedRadius);
        }

        /// <remarks>
        /// Called by animation event
        /// </remarks>
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            PlayRandomClip(defaultFootstepAudioClips, footstepByTileType);
        }

        /// <remarks>
        /// Called by animation event
        /// </remarks>
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            PlayRandomClip(defaultLandingAudioClips, landingByTileType);
        }

        private void PlayRandomClip(AudioClip[] defaultClips, AudioClipsForTileType[] clipsByTileType)
        {
            if (defaultClips.Length <= 0)
            {
                return;
            }

            AudioClip[] clips = null;
            GameState state = GameStateManager.Current;
            if (state && state.character != null && state.map != null)
            {
                clips = clipsByTileType.FirstOrDefault(f => f.tileType == state.map.GetTile(state.character.tile)?.config?.type)?.audioClips;
            }

            clips ??= defaultClips;

            int index = Random.Range(0, clips.Length);
            AudioSource.PlayClipAtPoint(clips[index], transform.TransformPoint(_controller.center), footstepAudioVolume);
        }
    }

    [Serializable]
    public class AudioClipsForTileType
    {
        public TileType tileType;
        public AudioClip[] audioClips;
    }
}
