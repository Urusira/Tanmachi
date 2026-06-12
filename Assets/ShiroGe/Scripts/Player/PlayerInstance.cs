using System;
using ShiroGe.Scripts.Inventory;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerInstance : MonoBehaviour
    {
        public static PlayerInstance Instance { get; private set; }
        private PlayerController _playerController;
        private PlayerInteractionController _playerInteraction;
        private CashManager _cashManager;
        private Camera _playerCamera;

        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            _playerCamera = Camera.main;
        }

        public void PlayerRegister(PlayerController playerController)
        {
            _playerController = playerController;
            _playerInteraction = playerController.GetComponent<PlayerInteractionController>();
            _cashManager = playerController.GetComponent<CashManager>();
        }

        public Vector3 GetPlayerWorldPosition()
        {
            return transform.position;
        }

        public Vector3 GetPlayerGroundedPointView()
        {
            return _playerInteraction.GlobalRaycastHit;
        }

        public Vector3 GetPlayerRawView()
        {
            return _playerInteraction.RawView;
        }
        
        public Vector3 GetPlayerForward()
        {
            Camera playerCamera = Camera.main;
            if (playerCamera != null) return playerCamera.transform.forward;
            else return _playerController.transform.forward;
        }

        public void GiveCash(int amount)
        {
            _cashManager.AddCash(amount);
        }
        
        public void RemoveCash(int amount)
        {
            _cashManager.RemoveCash(amount);
        }
        
        public void GiveItem(ItemSO item, int  amount)
        {
            InventoryManager.Instance.AddItem(item, amount);
        }
        
        public void RemoveItem(ItemSO item, int  amount)
        {
            InventoryManager.Instance.RemoveItem(item, amount);
        }
        
        public void CheckItem(ItemSO item, int minimalNeedAmount)
        {
            InventoryManager.Instance.InventoryItemWithAmountCheck(item, minimalNeedAmount);
        }
        
        public void TeleportPlayer(Vector3 position)
        {
            Debug.unityLogger.Log($"Teleporting player to {position}");
            _playerController.gameObject.transform.position = position;
        }

        public Vector3 GetPlayerRawStartPoint()
        {
            return _playerCamera.transform.position;
        }
    }
}