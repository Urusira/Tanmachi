using ShiroGe.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    
    [DefaultExecutionOrder(-2)]
    public class PlayerPlacementModeInput : MonoBehaviour, PlayerControls.IPlacementModeActions
    {
        [SerializeField] private ObjectPlacer _objectPlacer;
        
        private bool _hasRotationMode;
        private bool _fastRotation;
        
        public PlayerControls PlayerControls { get; private set; }
        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            
            PlayerControls.PlacementMode.Enable();
            PlayerControls.PlacementMode.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlacementMode.Disable();
            PlayerControls.PlacementMode.RemoveCallbacks(this);
        }
        
        public void OnPlace(InputAction.CallbackContext context)
        {
            if(!InventoryUiManager.Instance.InBuildingMode || !context.performed || InventoryUiManager.Instance.IsOpen) return;
            
            _objectPlacer.PlaceObject();
        }

        public void OnRotateMode(InputAction.CallbackContext context)
        {
            if (!InventoryUiManager.Instance.InBuildingMode || InventoryUiManager.Instance.IsOpen) return;
            
            if(context.started)
            {
                PlayerInputControllersRegulator.Instance.PlacementScrollLock();
                _hasRotationMode = true;
            }

            if (context.canceled)
            {
                PlayerInputControllersRegulator.Instance.PlacementScrollUnlock();
                _hasRotationMode = false;
            }
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if(!InventoryUiManager.Instance.InBuildingMode || !context.performed || !_hasRotationMode || InventoryUiManager.Instance.IsOpen) return;

            float scroll = context.ReadValue<Vector2>().y;
            
            _objectPlacer.RotateObject(scroll, _fastRotation);
        }

        public void OnFastRotation(InputAction.CallbackContext context)
        {
            if (!InventoryUiManager.Instance.InBuildingMode || InventoryUiManager.Instance.IsOpen) return;
            
            if (context.started)
            {
                _fastRotation = true;
            }
            
            if (context.canceled)
            {
                _fastRotation = false;
            }
        }

        public void OnEnterBuildMode(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            if(!InventoryUiManager.Instance.InBuildingMode)
            {
                InventoryUiManager.Instance.EnterBuildingMode();
            }
            else
            {
                InventoryUiManager.Instance.ExitBuildingMode();
            }
        }
    }
}