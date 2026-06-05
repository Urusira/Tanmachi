using System;
using ShiroGe.Scripts.NPC;
using UnityEngine;

public class NPCState : MonoBehaviour
{
    [field: SerializeField]
    public NPCMovementState CurrentNPCMovementState { get; private set; } = NPCMovementState.Idling;
    

    public void SetNPCActionsState(NPCMovementState newState)
    {
        CurrentNPCMovementState = newState;
    }

    public bool InGroundState()
    {
        return IsGroundedState(CurrentNPCMovementState);
    }

    public bool IsGroundedState(NPCMovementState movementState)
    {
        return movementState == NPCMovementState.Idling ||
               movementState == NPCMovementState.Walking ||
               movementState == NPCMovementState.Running ||
               movementState == NPCMovementState.Sprinting ||
               movementState == NPCMovementState.Strafing;
    }
}
