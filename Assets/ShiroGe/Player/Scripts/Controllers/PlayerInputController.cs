using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerInputController : MonoBehaviour, PlayerControls.IPlayerActions
    {
        [SerializeField] private bool holdToSprint = false;
        public PlayerControls PlayerControls { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        
        public bool JumpPressed { get; private set; }
        public bool AttackInput { get; private set; }
        public bool InteractInput { get; private set; }
        public bool SprintToggledOn { get; private set; }
        public bool WalkToggledOn { get; private set; }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            
            PlayerControls.Player.Enable();
            PlayerControls.Player.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.Player.Disable();
            PlayerControls.Player.RemoveCallbacks(this);
        }

        private void LateUpdate()
        {
            JumpPressed = false;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            AttackInput = context.performed;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            InteractInput = context.performed;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;
            
            JumpPressed = true;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn;
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
            }
        }

        public void OnWalkToggle(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;
            
            WalkToggledOn = !WalkToggledOn;
        }
    }
}