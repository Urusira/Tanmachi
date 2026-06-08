using System;
using System.Collections;
using JetBrains.Annotations;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NPCNavigator : MonoBehaviour
{
    public event System.Action OnDestinationReached;
    
    [SerializeField] private float movementThreesold = 0.01f;
    public float RotationMismatch { get; private set; } = 0f;
    public bool IsRotatingToTarget { get; private set; } = false;
    
    [Header("Движение")]
    public float walkAccel = 25f;
    public float walkMaxSpeed = 2f;
    public float runAccel = 35f;
    public float runMaxSpeed = 4f;
    public float sprintAccel = 50f;
    public float sprintMaxSpeed = 7f;
    
    public bool sprintToggledOn = false;
    public bool walkToggledOn = true;
    
    [Header("Блуждание")]
    public bool isWandering = false;
    public float wanderRadius = 100;
    public float wanderingIdleDuration = 10;
    
    [Header("Анимация")]
    public float playerModelRotationSpeed = 10f;
    public float rotateToTargetTime = 0.67f;
    
    private NavMeshAgent _navAgent;
    private NPCState _npcState;

    private Vector3 _destination;
    
    private bool _isRotatingClockwise = false;
    
    private float _rotatingToTargetTimer = 0f;

    private float currentWanderIdleTimer;
    
    private NPCMovementState _lastMovementState = NPCMovementState.Idling;

    void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _npcState = GetComponent<NPCState>();
    }

    private void Update()
    {
        HandleLateralMovement();
        UpdateMovementState();
        Wander();
        TargetReachedHandler();
    }

    private void UpdateMovementState()
    {
        _lastMovementState = _npcState.CurrentNPCMovementState;
        
        bool canRun = CanRun();
        bool isMovementInput = GetNPCPseudoInput() != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = sprintToggledOn && isMovingLaterally;
        bool isWalking = isMovingLaterally && (!canRun || walkToggledOn);
        bool isGrounded = IsGrounded();
        
        NPCMovementState movementState =    isWalking ? NPCMovementState.Walking : 
                                            isSprinting ? NPCMovementState.Sprinting :
                                            isMovingLaterally || isMovementInput ? NPCMovementState.Running : NPCMovementState.Idling;

        _npcState.SetNPCActionsState(movementState);
    }

    private bool IsGrounded()
    {
        return _npcState.InGroundState();
    }

    private void HandleLateralMovement()
    {
        bool isSprinting = _npcState.CurrentNPCMovementState == NPCMovementState.Sprinting;
        bool isGrounded = _npcState.InGroundState();
        bool isWalking = _npcState.CurrentNPCMovementState == NPCMovementState.Walking;

        float lateralAcceleration = isWalking ? walkAccel :
                                    isSprinting ? sprintAccel : runAccel;
        
        float clampLateralMagnitude =   isWalking ? walkMaxSpeed :
                                        isSprinting ? sprintMaxSpeed : runMaxSpeed;

        _navAgent.acceleration = lateralAcceleration;
        _navAgent.speed = clampLateralMagnitude;
    }

    private bool IsMovingLaterally()
    {
        Vector3 laterallyVelocity =  new Vector3(_navAgent.velocity.x, 0f, _navAgent.velocity.z);
            
        return laterallyVelocity.magnitude > movementThreesold;
    }
    
    /// <summary>
    /// Задаёт в AI навигаторе цель следования, куда агент будет двигаться, пока не достигнет цели
    /// </summary>
    /// <param name="target">Координаты точки, куда агент будет двигаться по NAVMesh-сетке</param>
    public bool MoveToTarget(Vector3 target)
    {
        _destination = target;
        return _navAgent.SetDestination(target);
    }

    /// <summary>
    /// Метод для случайного блуждания в определённом радиусе через промежутки времени, заданные в параметрах класса
    /// </summary>
    private void Wander()
    {
        if (!isWandering) return;
        
        if (_npcState.CurrentNPCMovementState == NPCMovementState.Idling)
        {
            currentWanderIdleTimer -= TimeManager.Instance.DeltaTime;

            if (currentWanderIdleTimer <= 0f)
            {
                MoveToTarget(GetRandomWanderTarget());
            }

            return;
        }

        if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance)
        {
            currentWanderIdleTimer = wanderingIdleDuration;
            
            return;
        }
    }

    /// <summary>
    /// Получает случайную точку в заданном параметром класса радиусе вокруг агента
    /// </summary>
    /// <returns>Спроецированная на NAV-сетку случайная позиция либо, при неудачной проекции, собственные координаты объекта</returns>
    private Vector3 GetRandomWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        return transform.position;
    }

    private void TargetReachedHandler()
    {
        if(_navAgent.enabled)
        {
            /*if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance &&
                _navAgent.velocity.sqrMagnitude < movementThreesold)
            {
                OnDestinationReached?.Invoke();
            }*/ //Устаревший способ определения достижения цели движения
            
            float distance = Vector3.Distance(transform.position, _destination);
            if (distance <= _navAgent.stoppingDistance)
            {
                OnDestinationReached?.Invoke();
            }
        }
    }

    public void LocomotionBlock()
    {
        _navAgent.isStopped = true;
        _navAgent.enabled = false;
    }

    public void LocomotionUnblock()
    {
        _navAgent.enabled = true;
        _navAgent.isStopped = false;

        //TODO: Заглушка чтоб дурачок не стоял
        isWandering = true;
    }

    public void FixLookAt(Vector3 target)
    {
        
    }

    public void UnfixLook()
    {
        
    }

    public Vector2 GetNPCPseudoInput()
    {
        Vector3 worldVelocity = _navAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
        Vector2 NPCPseudoInput = new Vector2(localVelocity.x, localVelocity.z).normalized;
        
        return NPCPseudoInput;
    }
    
    private bool CanRun()
    {
        return true;
    }
}
