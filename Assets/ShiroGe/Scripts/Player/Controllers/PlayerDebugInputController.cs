using System;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    
    [DefaultExecutionOrder(-2)]
    public class PlayerDebugInputController : MonoBehaviour,  PlayerControls.IDebugFunctionsActions
    {
        [SerializeField] private float teleportMaxDistance;
        public PlayerControls PlayerControls { get; private set; }
        
        private Camera _playerCamera;

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            
            PlayerControls.DebugFunctions.Enable();
            PlayerControls.DebugFunctions.SetCallbacks(this);
            
            _playerCamera = Camera.main;
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
            
            RaycastHit hit;
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward.normalized, out hit,
                    teleportMaxDistance, LayerManager.Instance.CollisiveLayers))
            {
                playerViewPoint = hit.point;
            }
            
            Debug.unityLogger.Log($"Teleport to {playerViewPoint}");
            PlayerInstance.Instance.TeleportPlayer(playerViewPoint);
        }
    }
}