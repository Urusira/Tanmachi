using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    
    [DefaultExecutionOrder(-2)]
    public class PlayerDebugInputController : MonoBehaviour,  PlayerControls.IDebugFunctionsActions
    {
        [SerializeField] private float teleportMaxDistance;
        [SerializeField] private LayerMask groundLayerMask;
        private Camera playerCamera;
        public PlayerControls PlayerControls { get; private set; }

        private void Start()
        {
            playerCamera = Camera.main;
        }

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
            
            if(playerCamera != null && playerCamera.isActiveAndEnabled)
            {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward.normalized,
                        out hit, teleportMaxDistance, groundLayerMask))
                {
                    Debug.unityLogger.Log($"Teleport to {hit.point}");
                    gameObject.transform.position = hit.point;
                }
            }
        }
    }
}