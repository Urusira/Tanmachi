using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Utils;
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

    private Vector3 _lookTarget = Vector3.zero;
    private Vector3 _destination;
    private Vector3 _prePausedDestination;
    public Vector3 _lastDestination  { get; private set; }
    public Vector3 _baseDestination { get; private set; }
    
    private bool _isRotatingClockwise = false;
    
    private float _rotatingToTargetTimer = 0f;

    private float currentWanderIdleTimer;

    private float repeatedPathUpdateTimerTime = 10f;
    
    RepeatedTimer repeatedPathUpdateTimer;
    
    private NPCMovementState _lastMovementState = NPCMovementState.Idling;

    void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _npcState = GetComponent<NPCState>();
    }

    private void Start()
    {
        repeatedPathUpdateTimer = new RepeatedTimer(repeatedPathUpdateTimerTime, PathUpdatingTimerOut);
    }

    private void Update()
    {
        HandleLateralMovement();
        UpdateMovementState();
        Wander();
        TargetReachedHandler();

        if (_lookTarget != Vector3.zero)
        {
            Vector3 rawDir = (_lookTarget - transform.position).normalized;
            Vector3 direction = new Vector3(rawDir.x, 0, rawDir.z);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            gameObject.transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.InOutSine);
        }
    }
    
    private void PathUpdatingTimerOut()
    {
        _navAgent.ResetPath();
        _navAgent.SetDestination(_destination);
    }

    public void SetBaseDestination(Vector3 baseDestination)
    {
        _baseDestination = baseDestination;
    }

    public bool GoToBaseDestination()
    {
        return MoveToTarget(_baseDestination);
    }
    
    public bool GoToLastDestination()
    {
        return MoveToTarget(_lastDestination);
    }

    public void ResetCurrentDestination(bool withRemembering = false)
    {
        if (withRemembering)
        {
            _lastDestination = _destination;
        }
        _destination = Vector3.zero;

    }

    public void SetRandomAvoidancyPrio()
    {
        _navAgent.avoidancePriority = Random.Range(0, 99);
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
    /// Задаёт в AI навигаторе цель следования, куда агент будет двигаться, пока не достигнет цели.
    /// При неудаче агент продолжает двигаться к прошлой точке. Если она не доступна - двигается к стандартной цели. При её отсутствии отправляется бродить.
    /// </summary>
    /// <param name="target">Координаты точки, куда агент будет двигаться по NAVMesh-сетке</param>
    public bool MoveToTarget(Vector3 target)
    {
        if(_destination != Vector3.zero && _destination != _prePausedDestination) _lastDestination = _destination;
        
        
        bool successful = TrySetDestination(
            newTarget: target, 
            "Cannot move to target destination point, going to last destination point", 
            "Cannot move to zero coordinates");
        if (successful) return true;

        bool lastSuccessful = TrySetDestination(
            newTarget: _lastDestination, 
            "Cannot move to last destination point, going to base destination point", 
            "Last destination point is null");
        
        if (!lastSuccessful)
        {
            bool baseSuccessful = TrySetDestination(
                newTarget: _baseDestination,
                "Cannot move to base destination point, going to wandering", 
                "Base destination point is null");
            
            if (!baseSuccessful)
            {
                _destination = Vector3.zero;
                _lastDestination = Vector3.zero;
                
                isWandering = true;
            }
        }
        
        return false;
    }

    private bool TrySetDestination(Vector3 newTarget, string errorMessage, string nullErrorMessage)
    {
        bool successful = false;
        
        if(newTarget != Vector3.zero)
        {
            _destination = newTarget;
            successful = _navAgent.SetDestination(_destination);
        }
        else
        {
            Debug.LogWarning(nullErrorMessage);
        }
        
        if(!successful) Debug.LogWarning(errorMessage);

        return successful;
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
        _prePausedDestination = _destination;
        _navAgent.isStopped = true;
        _navAgent.enabled = false;
    }

    public void LocomotionUnblock(bool goToPrepausedPosition)
    {
        _navAgent.enabled = true;
        _navAgent.isStopped = false;
        if(goToPrepausedPosition) MoveToTarget(_prePausedDestination);
    }

    public void FixLookAt(Vector3 target)
    {
        _lookTarget = target;
    }

    public void UnfixLook()
    {
        _lookTarget = Vector3.zero;
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

    public void SetLastDestinationPoint(Vector3 newLastDestination)
    {
        _lastDestination = newLastDestination;
    }
}
