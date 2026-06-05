    using ShiroGe.Scripts.NPC;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float locomotionBlendSpeed = 20f;
    
    private NPCNavigator _npcNavigator;
    private NPCState _npcState;
    
    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
    private static int isGroundedHash = Animator.StringToHash("isGrounded");
    private static int isIdlingHash = Animator.StringToHash("isIdling");
    private static int isRotatingToTargetHash =  Animator.StringToHash("isRotatingToTarget");
    private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");
    private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");

    private Vector3 _currentBlendInput = Vector3.zero;

    private float _sprintMaxBlendValue = 1.5f;
    private float _runMaxBlendValue = 1.0f;
    private float _walkMaxBlendValue = 0.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _npcNavigator = GetComponent<NPCNavigator>();
        _npcState = GetComponent<NPCState>();
    }

    private void Update()
    {
        UpdateAnimState();
    }

    private void UpdateAnimState()
    {
        bool isIdling = _npcState.CurrentNPCMovementState == NPCMovementState.Idling;
        bool isRunning = _npcState.CurrentNPCMovementState == NPCMovementState.Running;
        bool isSprinting = _npcState.CurrentNPCMovementState == NPCMovementState.Sprinting;
        bool isJumping = _npcState.CurrentNPCMovementState == NPCMovementState.Jumping;
        bool isFalling = _npcState.CurrentNPCMovementState == NPCMovementState.Falling;
        bool isGrounded = _npcState.InGroundState();
        
        bool isRunningBlendValue = isRunning || isJumping || isFalling;
        Vector2 inputTarget =    isSprinting ? _npcNavigator.GetNPCPseudoInput() * _sprintMaxBlendValue :
                                    isRunningBlendValue ? _npcNavigator.GetNPCPseudoInput() * _runMaxBlendValue : 
                                                            _npcNavigator.GetNPCPseudoInput() * _walkMaxBlendValue;
        //Debug.Log(inputTarget);
        _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
            
        animator.SetBool(isGroundedHash, isGrounded);
        animator.SetBool(isIdlingHash, isIdling);
        animator.SetBool(isRotatingToTargetHash, _npcNavigator.IsRotatingToTarget);
            
        animator.SetFloat(inputXHash, _currentBlendInput.x);
        animator.SetFloat(inputYHash, _currentBlendInput.y);
        animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
        animator.SetFloat(rotationMismatchHash, _npcNavigator.RotationMismatch);
    }
}
