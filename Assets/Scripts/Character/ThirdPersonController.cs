﻿using System;
using System.Linq;
using Character.Player;
using Map;
using Map.Tile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Character
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        public AudioClipsForTileType[] landingByTileType;
        public AudioClip[] defaultLandingAudioClips;
        public AudioClipsForTileType[] footstepByTileType;
        public AudioClip[] defaultFootstepAudioClips;
        [Range(0, 1)]
        public float footstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        [Header("Camera")]
        public Transform cameraTarget;
        public float cameraRotationSmoothTime = 0.1f;
        public float cameraAngle;
        public float zoomSpeed = 0.1f;
        public float zoomLevel;

        [Header("Events")]
        public UnityEvent onMoveStart = new();
        public UnityEvent onMoveEnd = new();

        // player
        private bool _isSprinting;
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
        private PlayerControllerInputSource _playerControllerInputSource;
        private GameObject _mainCamera;

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
            _playerControllerInputSource = GetComponent<PlayerControllerInputSource>();
            GetComponent<PlayerInput>();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            UpdateCamera();
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
            Vector3 spherePosition = new(position.x, position.y - groundedOffset, position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

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
                _isSprinting = _playerControllerInputSource.sprint;
            }
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _isSprinting ? sprintSpeed : moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_playerControllerInputSource.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            Vector3 velocity = _controller.velocity;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _playerControllerInputSource.analogMovement ? _playerControllerInputSource.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_playerControllerInputSource.move.x, 0.0f, _playerControllerInputSource.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_playerControllerInputSource.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

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
                _fallTimeoutDelta = fallTimeout;

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
                if (_playerControllerInputSource.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

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
                _jumpTimeoutDelta = jumpTimeout;

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
                _playerControllerInputSource.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void UpdateCamera()
        {
            cameraAngle = Mathf.SmoothDampAngle(
                              cameraAngle,
                              cameraAngle + _playerControllerInputSource.look * 10,
                              ref _cameraRotationVelocity,
                              cameraRotationSmoothTime
                          )
                          % 360;

            float targetZoom = _playerControllerInputSource.zoom > 0.5
                ? 1
                : _playerControllerInputSource.zoom < -0.5
                    ? 0
                    : zoomLevel;
            zoomLevel = Mathf.SmoothStep(zoomLevel, targetZoom, zoomSpeed);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Vector3 position = transform.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - groundedOffset, position.z), groundedRadius);
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
            if (state && state.player != null && state.map != null)
            {
                clips = clipsByTileType.FirstOrDefault(f => f.tileType == state.map.GetTile(state.player.playerTile)?.config?.type)?.audioClips;
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
