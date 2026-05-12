using UnityEngine;

namespace ShiroGe.CharacterController 
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;
        [field: SerializeField]
        public PlayerActionsState CurrentPlayerActionsState { get; private set; } = PlayerActionsState.Default;
        
        public bool _inDialogState { get; private set; } = false;

        public void SetPlayerMovementState(PlayerMovementState newState)
        {
            CurrentPlayerMovementState = newState;
        }

        public void SetPlayerActionsState(PlayerActionsState newState)
        {
            CurrentPlayerActionsState = newState;
        }

        public void SetPlayerActionStateDefault()
        {
            CurrentPlayerActionsState = PlayerActionsState.Default;
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

        public bool InDialogChange()
        {
            _inDialogState = !_inDialogState;
            return _inDialogState;
        }
    }
}