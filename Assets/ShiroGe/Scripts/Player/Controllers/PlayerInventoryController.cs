using ShiroGe.Scripts.UI;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerInventoryController : MonoBehaviour, PlayerControls.IInventoryActions
    {
        public PlayerControls PlayerControls { get; private set; }

        public bool scrollLock;
        public bool forcedDrop;

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.Inventory.Enable();
            PlayerControls.Inventory.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.Inventory.Disable();
            PlayerControls.Inventory.RemoveCallbacks(this);
        }
        
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            InventoryUiManager.Instance.LeftClick();
        }

        public void OnToggleInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if(!InventoryUiManager.Instance.IsOpen)
                InventoryUiManager.Instance.ShowInventory();
            else
                InventoryUiManager.Instance.HideInventory();
        }

        public void OnDropItem(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            if(!InventoryUiManager.Instance.IsOpen)
            {
                GameObject droppedItem = InventoryManager.Instance.DropItem(false);

                if (droppedItem == null) return;

                if (forcedDrop)
                    WorldSpawner.Instance.PlayerDrop(droppedItem, PlayerStats.Instance.ThrowForce, true);
                else
                    WorldSpawner.Instance.PlayerDrop(droppedItem, PlayerStats.Instance.DropForce);
            }
            else
            {
                GameObject droppedItem = InventoryManager.Instance.DropItem(true);
                
                if (droppedItem == null) return;
                
                WorldSpawner.Instance.PlayerDrop(droppedItem, PlayerStats.Instance.DropForce);
            }
        }

        public void OnQuickTransferItem(InputAction.CallbackContext context)
        {
            if(InventoryUiManager.Instance.IsOpen)
            {
                if (context.started || context.canceled)
                    InventoryUiManager.Instance.SetQuickTransfer();
            }
            else 
            {
                if(context.started)
                    forcedDrop = true;
                if(context.canceled)
                    forcedDrop = false;
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if(!context.performed  || !InventoryUiManager.Instance.IsOpen) return;
            
            InventoryUiManager.Instance.RightClick();
        }

        #region HotbarSelections
        public void OnHotbar1(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(0);
        }

        public void OnHotbar2(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(1);
        }

        public void OnHotbar3(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(2);
        }

        public void OnHotbar4(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(3);
        }

        public void OnHotbar5(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(4);
        }

        public void OnHotbar6(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(5);
        }

        public void OnHotbar7(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(6);
        }

        public void OnHotbar8(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(7);
        }

        public void OnHotbar9(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            InventoryUiManager.Instance.HotbarSelect(8);
        }

        public void OnItemsScroll(InputAction.CallbackContext context)
        {
            if(!context.performed || scrollLock) return;

            float scroll = context.ReadValue<Vector2>().y;
            
            if(scroll > 0)
            {
                InventoryUiManager.Instance.PreviousItem();
                return;
            }
            if(scroll < 0)
            {
                InventoryUiManager.Instance.NextItem();
                return;
            }
        }
        #endregion
    }
}