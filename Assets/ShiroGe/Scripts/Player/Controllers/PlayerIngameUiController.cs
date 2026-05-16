using System;
using ShiroGe.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ShiroGe.CharacterController
{
    public class PlayerIngameUiController: MonoBehaviour, PlayerControls.IIngameUIActions
    {
        public PlayerControls PlayerControls { get; private set; }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.IngameUI.Enable();
            PlayerControls.IngameUI.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.IngameUI.Disable();
            PlayerControls.IngameUI.RemoveCallbacks(this);
        }
        
        public void IngameUiActionsDisable()
        {
            PlayerControls.IngameUI.Disable();
        }
        
        public void IngameUiActionsEnable()
        {
            PlayerControls.IngameUI.Enable();
        }
        
        //TODO: ВИДИШЬ ГОВНОКОД. РЕФАКТОРЬ. (ТУТ ГОВНО, ВНИМАНИЕ, ТРЕБУЕТСЯ ПЕРЕДЕЛКА, ВНИМАНИЕ, ПЕРЕДЕЛКА СРОЧНО ТРЕБУЕТСЯ ПЕРЕДЕЛКА)
        //Как вариант сделать абстракцию или интерфейс, откуда будут наследоваться все интерфейсы, где будет метод Hide(), который можно вызывать...
        //Тогда надо хранить где-то текущее открытое окно, например в гуи менеджере, уже отсюда будет просто вызываться текущее открытое окно из гуименеджера и .Hide()
        public void OnBackToMenu(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if(InventoryUiManager.Instance.IsOpen)
            {
                InventoryUiManager.Instance.HideInventory();
                return;
            }
            if(DialogManager.Instance.InDialog)
            {
                DialogManager.Instance.CloseDialog();
                return;
            }
            SceneManager.UnloadSceneAsync("GameMainScene");
            SceneManager.LoadScene("MainMenuScene");
        }

        public void OnOpenInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (!InventoryUiManager.Instance.IsOpen)
            {
                InventoryUiManager.Instance.ShowInventory();
            }
            else
            {
                InventoryUiManager.Instance.HideInventory();
            }
        }
    }
}