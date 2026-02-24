using UnityEngine;

namespace ShiroGe.CharacterController 
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

        public void SetPlayerMovementState(PlayerMovementState newState)
        {
            CurrentPlayerMovementState = newState;
        }

        public bool InGroundState()
        {
            return IsGroundedState(CurrentPlayerMovementState);
        }

        public bool IsGroundedState(PlayerMovementState movementState)
        {
            return movementState == PlayerMovementState.Idling ||
                   movementState == PlayerMovementState.Walking ||
                   movementState == PlayerMovementState.Running ||
                   movementState == PlayerMovementState.Sprinting ||
                   movementState == PlayerMovementState.Strafing;
        }
    }
}