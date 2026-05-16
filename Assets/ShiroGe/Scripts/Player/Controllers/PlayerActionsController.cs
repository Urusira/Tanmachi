using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerActionsController : MonoBehaviour, PlayerControls.IPlayerActionsActions
    {
        public PlayerControls PlayerControls { get; private set; }
        public bool AttackInput { get; private set; }
        public bool InteractInput { get; private set; }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.PlayerActions.Enable();
            PlayerControls.PlayerActions.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlayerActions.Disable();
            PlayerControls.PlayerActions.RemoveCallbacks(this);
        }
        
        public void ActionsDisable()
        {
            PlayerControls.PlayerActions.Disable();
        }
        
        public void ActionsEnable()
        {
            PlayerControls.PlayerActions.Enable();
        }

        public void SetAttackPressedFalse()
        {
            AttackInput = false;
        }

        public void SetInteractPressedFalse()
        {
            InteractInput = false;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            AttackInput = true;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            InteractInput = true;
        }
    }
}