using System;
using ShiroGe.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ShiroGe.CharacterController
{
    
    [DefaultExecutionOrder(-2)]
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
        
        //TODO: ВИДИШЬ ГОВНОКОД. РЕФАКТОРЬ. (ТУТ ГОВНО, ВНИМАНИЕ, ТРЕБУЕТСЯ ПЕРЕДЕЛКА, ВНИМАНИЕ, ПЕРЕДЕЛКА СРОЧНО ТРЕБУЕТСЯ ПЕРЕДЕЛКА)
        //Как вариант сделать абстракцию или интерфейс, откуда будут наследоваться все интерфейсы, где будет метод Hide(), который можно вызывать...
        //Тогда надо хранить где-то текущее открытое окно, например в гуи менеджере, уже отсюда будет просто вызываться текущее открытое окно из гуименеджера и .Hide()
        public void OnBackToMenu(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if(DialogManager.Instance.InDialog)
            {
                DialogManager.Instance.CloseDialog();
                return;
            }

            if (InventoryUiManager.Instance.IsOpen)
            {
                InventoryUiManager.Instance.HideInventory();
                return;
            }
            
            SceneManager.UnloadSceneAsync("GameMainScene");
            SceneManager.LoadScene("MainMenuScene");
        }

        public void OnHelp(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            GuiManager.Instance.SwitchControlHint();
        }
    }
}