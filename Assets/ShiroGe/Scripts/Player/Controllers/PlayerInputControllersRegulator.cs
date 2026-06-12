using System;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    [RequireComponent(typeof(PlayerInputController))]
    [RequireComponent(typeof(PlayerIngameUiController))]
    [RequireComponent(typeof(PlayerActionsController))]
    [RequireComponent(typeof(PlayerInventoryController))]
    [RequireComponent(typeof(PlayerDebugInputController))]
    [RequireComponent(typeof(PlayerPlacementModeInput))]
    public class PlayerInputControllersRegulator : MonoBehaviour
    {
        public static PlayerInputControllersRegulator Instance  { get; private set; }

        private PlayerInputController _playerInputController;
        private PlayerIngameUiController _playerIngameUiController;
        private PlayerActionsController _playerActionsController;
        private PlayerInventoryController _playerInventoryController;
        private PlayerDebugInputController _playerDebugInputController;
        private PlayerPlacementModeInput  _playerPlacementModeInput;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            TryGetComponent(out _playerInputController);
            TryGetComponent(out _playerIngameUiController);
            TryGetComponent(out _playerActionsController);
            TryGetComponent(out _playerInventoryController);
            TryGetComponent(out _playerDebugInputController);
            TryGetComponent(out _playerPlacementModeInput);
            
        }

        /*public void EnterPlacementMode()
        {
            _playerPlacementModeInput.PlayerControls.PlacementMode.Enable();
            _playerActionsController.PlayerControls.PlayerActions.Disable();
        }
        public void ExitPlacementMode()
        {
            _playerPlacementModeInput.PlayerControls.PlacementMode.Disable();
            _playerActionsController.PlayerControls.PlayerActions.Enable();
        }*/

        public void DisableMovement()
        {
            _playerInputController.PlayerControls.PlayerMovement.Disable();
        }
        public void EnableMovement()
        {
            _playerInputController.PlayerControls.PlayerMovement.Enable();
        }

        public void DisableInventoryControls()
        {
            _playerInventoryController.PlayerControls.Inventory.Disable();
        }
        public void EnableInventoryControls()
        {
            _playerInventoryController.PlayerControls.Inventory.Enable();
        }
        
        public void DisableIngameUiActions()
        {
            _playerIngameUiController.PlayerControls.IngameUI.Disable();
        }
        public void EnableIngameUiActions()
        {
            _playerIngameUiController.PlayerControls.IngameUI.Enable();
        }

        public void DisablePlayerActions()
        {
            _playerActionsController.PlayerControls.PlayerActions.Disable();
        }
        public void EnablePlayerActions()
        {
            _playerActionsController.PlayerControls.PlayerActions.Enable();
        }

        public void DisableDebugCheats()
        {
            _playerDebugInputController.PlayerControls.DebugFunctions.Disable();
        }
        public void EnableDebugCheats()
        {
            _playerDebugInputController.PlayerControls.DebugFunctions.Enable();
        }

        public void PlacementScrollLock()
        {
            _playerInventoryController.scrollLock = true;
        }
        public void PlacementScrollUnlock()
        {
            _playerInventoryController.scrollLock = false;
        }
    }
}