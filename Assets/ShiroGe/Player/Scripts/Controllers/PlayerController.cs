using System;
using System.Numerics;
using ShiroGe.Scripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private UnityEngine.CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatingToTarget { get; private set; } = false;
        
        [Header("Movement")]
        public float walkAccel = 25f;
        public float walkMaxSpeed = 2f;
        public float runAccel = 35f;
        public float runMaxSpeed = 4f;
        public float sprintAccel = 50f;
        public float sprintMaxSpeed = 7f;
        public float inAirAccel = 20f;
        public float jumpSpeed = 1.0f;
        public float drag = 20f;
        public float movingThreesold = 0.01f;
        
        [Header("Camera")]
        public float cameraSensitivityH = .1f;
        public float cameraSensitivityV = .1f;
        public float lookLimitV = 89f;
        
        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.67f;
        
        [Header("World")]
        public float gravity = 25f;
        
        [Header("Environment")]
        [SerializeField] private LayerMask _groundLayers;
        
        [Header("Rofl")]
        public bool slowedStrafe = false;
        public bool superJumps = false;
        
        private PlayerInputController _playerInputContoller;
        private PlayerState _playerState;
        
        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private bool _isRotatingClockwise = false;
        private bool _jumpedLastFrame = false;
        
        private float _rotatingToTargetTimer = 0f;
        private float _verticalVelocity = 0f;
        private float _antiBumpSpeed;
        private float _stepOffset;

        private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;
        LayerMask _layerMask;
        
        
        
        GameObject _target;
        
        [SerializeField, InspectorName("Дальность взаимодействия")]
        private float interactionDistance = 3f;
        #endregion

        
        #region Startup
        private void Awake()
        {
            _playerInputContoller = GetComponent<PlayerInputController>();
            _playerState = GetComponent<PlayerState>();

            _antiBumpSpeed = sprintMaxSpeed;
            _stepOffset = _characterController.stepOffset;
        }
        private void Start()
        {
            _layerMask = LayerMask.GetMask("Interactable");
        }
        #endregion
        
        #region Update Logic
        private void Update()
        {
            UpdateMovementState();
            HandleVerticalMovement();
            HandleLateralMovement();
            PointerScan();
        }

        private void UpdateMovementState()
        {
            _lastMovementState = _playerState.CurrentPlayerMovementState;
            
            bool canRun = CanRun();
            bool isMovementInput = _playerInputContoller.MovementInput != Vector2.zero;
            bool isMovingLaterally = IsMovingLaterally();
            bool isSprinting = _playerInputContoller.SprintToggledOn && isMovingLaterally;
            bool isWalking = isMovingLaterally && (!canRun || _playerInputContoller.WalkToggledOn);
            bool isGrounded = IsGrounded();

            PlayerMovementState lateralState =  isWalking ? PlayerMovementState.Walking : 
                                                isSprinting ? PlayerMovementState.Sprinting :
                                                isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
            _playerState.SetPlayerMovementState(lateralState);

            if ((!isGrounded || (_jumpedLastFrame && !superJumps)) && _characterController.velocity.y > 0)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
                _jumpedLastFrame = false;
                _characterController.stepOffset = 0f;
            }
            else if ((!isGrounded || (_jumpedLastFrame && !superJumps)) && _characterController.velocity.y <= 0)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
                _jumpedLastFrame = false;
                _characterController.stepOffset = 0f;
            }
            else
            {
                _characterController.stepOffset = _stepOffset;
            }
        }

        private void HandleVerticalMovement()
        {
            bool isGrounded = _playerState.InGroundState();

            _verticalVelocity -= gravity * Time.deltaTime;
            
            if (isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -_antiBumpSpeed;
            

            if (_playerInputContoller.JumpPressed && isGrounded)
            {
                _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
                _jumpedLastFrame = true;
            }

            if (_playerState.IsGroundedState(_lastMovementState) && !isGrounded)
            {
                _verticalVelocity += _antiBumpSpeed;
            }
        }

        private void HandleLateralMovement()
        {
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = _playerState.InGroundState();
            bool isWalking = _playerState.CurrentPlayerMovementState == PlayerMovementState.Walking;

            float lateralAcceleration = !isGrounded ? inAirAccel : 
                                        isWalking ? walkAccel :
                                        isSprinting ? sprintAccel : runAccel;
            float clampLateralMagnitude =   !isGrounded ? sprintMaxSpeed :
                                            isWalking ? walkMaxSpeed :
                                            isSprinting ? sprintMaxSpeed : runMaxSpeed;
            
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0, _playerCamera.transform.right.z).normalized;
            Vector3 moveDirection = cameraRightXZ * _playerInputContoller.MovementInput.x +
                                    cameraForwardXZ * _playerInputContoller.MovementInput.y;
            
            Vector3 movementDelta = moveDirection * lateralAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;
            
            Vector3 curDrag = newVelocity.normalized * drag *  Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - curDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
            newVelocity.y += _verticalVelocity;
            newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;
            
            _characterController.Move(newVelocity * Time.deltaTime);
        }

        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = PlayerControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= _characterController.slopeLimit;

            if (!validAngle && _verticalVelocity < 0f)
            {
                velocity = Vector3.ProjectOnPlane(velocity, normal);
            }
            
            return velocity;
        }
        
        //TODO: Needs Refactor
        private void PointerScan()
        {
            RaycastHit hit;
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward.normalized, out hit,
                    interactionDistance, _layerMask))
            {
                _target = hit.collider.gameObject;
                _target?.GetComponent<Interactable>().ShowHint();
                GuiManager.Instance.HighlightPointer();
            }
            else
            {
                _target?.GetComponent<Interactable>().HideHint();
                _target = null;
                GuiManager.Instance.ResetPointer();
            }

            if (_playerInputContoller.InteractInput)
                _target?.GetComponent<Interactable>().Interact();
        }

        #endregion

        #region Lateupdate Logic
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation()
        {
            _cameraRotation.x += cameraSensitivityH * _playerInputContoller.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - cameraSensitivityV * _playerInputContoller.LookInput.y, -lookLimitV, lookLimitV);
            
            _playerTargetRotation.x += transform.eulerAngles.x + cameraSensitivityH * _playerInputContoller.LookInput.x;

            float rotationTolerance = 90f;
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            IsRotatingToTarget = _rotatingToTargetTimer > 0;
            if (!isIdling)
            {
                RotatePlayerToTarget();
            }
            else if(Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
            {
                UpdateIdleRotation(rotationTolerance);
            }
            
            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
            
            Vector3 camForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
            float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
            RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
        }

        private void UpdateIdleRotation(float rotationTolerance) {
            if (Mathf.Abs(RotationMismatch) > rotationTolerance)
            {
                _rotatingToTargetTimer = rotateToTargetTime;
                _isRotatingClockwise = RotationMismatch > rotationTolerance;
            }
            _rotatingToTargetTimer -= Time.deltaTime;

            if (_isRotatingClockwise && RotationMismatch > 0f || !_isRotatingClockwise &&  RotationMismatch < 0f)
            {
                RotatePlayerToTarget();
            }
        }
        
        private void RotatePlayerToTarget()
        {
            Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
        }
        
        #endregion
        
        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 laterallyVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
            
            return laterallyVelocity.magnitude > movingThreesold;
        }
        
        private bool IsGrounded()
        {
            return _playerState.InGroundState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();
        }

        private bool IsGroundedWhileGrounded()
        {
            Vector3 normal = PlayerControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= _characterController.slopeLimit;
            
            Vector3 spherePosition = new Vector3(transform.position.x,
                transform.position.y - _characterController.radius, transform.position.z);
            bool grounded = Physics.CheckSphere(spherePosition, _characterController.radius, _groundLayers, QueryTriggerInteraction.Ignore);

            return grounded & validAngle;
        }

        private bool IsGroundedWhileAirborne()
        {
            Vector3 normal = PlayerControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= _characterController.slopeLimit;
            
            return _characterController.isGrounded && validAngle;
        }

        private bool CanRun()
        {
            if (slowedStrafe)
                return _playerInputContoller.MovementInput.y >= Mathf.Abs(_playerInputContoller.MovementInput.x);
            
            return true;
        }
        #endregion
    }
}