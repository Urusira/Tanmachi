using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.UI
{
    public class InventoryUiManager : MonoBehaviour
    {
        public static InventoryUiManager Instance { get; private set; }
        
        [SerializeField] private GameObject inventoryObj;
        [SerializeField] private GameObject playerObj;
        
        [SerializeField] private GameObject selectorBorderObj;
        
        private PlayerController _playerController;
        
        public bool IsOpen { get; private set; } = false;

        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            //DontDestroyOnLoad(gameObject);

            Instance = this;
            
            inventoryObj.SetActive(false);
            
            _playerController = playerObj.GetComponent<PlayerController>();
        }

        public void ShowInventory()
        {
            GuiManager.Instance.HideGui();
            GuiManager.Instance.UnlockMouse();
            _playerController.LockControl();
            
            inventoryObj.SetActive(true);
            IsOpen = true;
        }

        public void HideInventory()
        {
            InventoryManager.Instance.InventoryClosedHandler();
            
            GuiManager.Instance.ShowGui();
            GuiManager.Instance.LockMouse();
            _playerController.UnlockControl();
            
            inventoryObj.SetActive(false);
            IsOpen = false;
        }

        public void LeftClick()
        {
            InventoryManager.Instance.DragAndDrop();
        }

        public void RightClick()
        {
            InventoryManager.Instance.DragAndDrop(half: true);
        }

        public void SetQuickTransfer()
        {
            InventoryManager.Instance.SetQuickTransfer();
        }

        public void HotbarSelect(int value)
        {
            InventoryManager.Instance.HotbarSelectorUpdate(value);
            selectorBorderObj.transform.position = new Vector3(
                InventoryManager.Instance._hotbarSlots[InventoryManager.Instance.SelectedHotbarSlot].transform.position.x, 
                selectorBorderObj.transform.position.y, 
                selectorBorderObj.transform.position.z
                );
        }
        
        public void NextItem()
        {
            HotbarSelect(InventoryManager.Instance.SelectedHotbarSlot+1);
        }
        
        public void PrevoiusItem()
        {
            HotbarSelect(InventoryManager.Instance.SelectedHotbarSlot-1);
        }
    }
}