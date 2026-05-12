using System;
using System.Linq;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = .02f;
        
        private PlayerInputController _inputController;
        private PlayerState _playerState;
        private PlayerController _playerController;
        private PlayerActionsController _playerActionsController;
        
        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");
        private static int isIdlingHash = Animator.StringToHash("isIdling");
        private static int isRotatingToTargetHash =  Animator.StringToHash("isRotatingToTarget");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");
        private static int isAttackingHash = Animator.StringToHash("isAttacking");
        private static int isPickingUpHash = Animator.StringToHash("isPickingUp");
        private static int Handling1HHorizontalHash = Animator.StringToHash("isHandling1HHorizontal");
        private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");
        private int[] actionHashes;
        
        private Vector3 _currentBlendInput = Vector3.zero;

        private float _sprintMaxBlendValue = 1.5f;
        private float _runMaxBlendValue = 1.0f;
        private float _walkMaxBlendValue = 0.5f;

        private void Awake()
        {
            _inputController = GetComponent<PlayerInputController>();
            _playerState = GetComponent<PlayerState>();
            _playerController = GetComponent<PlayerController>();
            _playerActionsController = GetComponent<PlayerActionsController>();

            //actionHashes = new[] { }; //Сюда добавляем хеши прерываемых анимаций. Пустой, т.к. ни одна из анимаций у меня не прерывается
        }

        private void Update()
        {
            UpdateAnimState();
        }

        private void UpdateAnimState()
        {
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isHandling1HHorizontal = _playerState.CurrentPlayerActionsState == PlayerActionsState.Handling1HHorizontal;
            bool isPickingUp = _playerState.CurrentPlayerActionsState == PlayerActionsState.PickingUp;
            bool isGrounded = _playerState.InGroundState();
            //bool isPlayingAction = actionHashes.Any(hash => _animator.GetBool(hash));

            bool isRunningBlendValue = isRunning || isJumping || isFalling;
            Vector2 inputTarget =   isSprinting ? _inputController.MovementInput * _sprintMaxBlendValue :
                                    isRunningBlendValue ? _inputController.MovementInput * _runMaxBlendValue : 
                                                            _inputController.MovementInput * _walkMaxBlendValue;
            
            inputTarget = !_playerState._inDialogState ? inputTarget : Vector2.zero;
            
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
            
            _animator.SetBool(isGroundedHash, isGrounded);
            _animator.SetBool(isIdlingHash, isIdling);
            _animator.SetBool(isFallingHash, isFalling);
            _animator.SetBool(isJumpingHash, isJumping);
            _animator.SetBool(Handling1HHorizontalHash, isHandling1HHorizontal);
            _animator.SetBool(isPickingUpHash, isPickingUp);
            _animator.SetBool(isRotatingToTargetHash, _playerController.IsRotatingToTarget);
            _animator.SetBool(isAttackingHash, _playerActionsController.AttackInput);
            //_animator.SetBool(isPlayingActionHash, isPlayingAction);
            
            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        }
    }
}