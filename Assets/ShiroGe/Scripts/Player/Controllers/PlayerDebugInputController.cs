using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    
    [DefaultExecutionOrder(-2)]
    public class PlayerDebugInputController : MonoBehaviour,  PlayerControls.IDebugFunctionsActions
    {
        [SerializeField] private float teleportMaxDistance;
        public PlayerControls PlayerControls { get; private set; }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            
            PlayerControls.DebugFunctions.Enable();
            PlayerControls.DebugFunctions.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.DebugFunctions.Disable();
            PlayerControls.DebugFunctions.RemoveCallbacks(this);
        }
        
        public void OnTeleport(InputAction.CallbackContext context)
        {
            if(!context.performed) return;

            Vector3 playerViewPoint = PlayerInstance.Instance.GetPlayerGroundedPointView();
            
            Debug.unityLogger.Log($"Teleport to {playerViewPoint}");
            PlayerInstance.Instance.TeleportPlayer(playerViewPoint);
        }
    }
}