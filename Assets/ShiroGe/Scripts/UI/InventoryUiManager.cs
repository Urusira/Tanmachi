using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.UI
{
    public class InventoryUiManager : MonoBehaviour
    {
        public static InventoryUiManager Instance { get; private set; }
        
        [SerializeField] private GameObject inventoryObj;
        [SerializeField] private GameObject playerObj;
        
        private PlayerController _playerController;
        
        public bool IsOpen { get; private set; } = false;

        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);

            Instance = this;
            
            inventoryObj.SetActive(false);
            
            _playerController = playerObj.GetComponent<PlayerController>();
        }

        public void ShowInventory()
        {
            GuiManager.Instance.UnlockMouse();
            _playerController.LockControl();
            
            inventoryObj.SetActive(true);
            IsOpen = true;
        }

        public void HideInventory()
        {
            GuiManager.Instance.LockMouse();
            _playerController.UnlockControl();
            
            inventoryObj.SetActive(false);
            IsOpen = false;
        }
    }
}