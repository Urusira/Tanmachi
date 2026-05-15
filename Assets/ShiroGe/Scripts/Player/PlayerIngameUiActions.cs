using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ShiroGe.CharacterController
{
    public class PlayerIngameUiActions: MonoBehaviour, PlayerControls.IIngameUIActions
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
        
        public void OnBackToMenu(InputAction.CallbackContext context)
        {
            SceneManager.UnloadSceneAsync("GameMainScene");
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}