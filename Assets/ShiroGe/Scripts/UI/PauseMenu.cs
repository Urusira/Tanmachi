using ShiroGe.CharacterController;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShiroGe.Scripts.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }
        
        
        [SerializeField] private GameObject pauseMenu;
        public bool Paused { get; private set; } = false;

        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            pauseMenu.SetActive(false);
        }

        public void ShowMenu()
        {
            PlayerInputControllersRegulator.Instance.DisableMovement();
            PlayerInputControllersRegulator.Instance.DisableInventoryControls();
            PlayerInputControllersRegulator.Instance.DisableDebugCheats();
            PlayerInputControllersRegulator.Instance.DisablePlayerActions();
            GuiManager.Instance.UnlockMouse();
            TimeManager.Instance.TimeStop();
            Paused = true;
            pauseMenu.SetActive(true);
        }
        
        public void HideMenu()
        {
            PlayerInputControllersRegulator.Instance.EnableMovement();
            PlayerInputControllersRegulator.Instance.EnableInventoryControls();
            PlayerInputControllersRegulator.Instance.EnableDebugCheats();
            PlayerInputControllersRegulator.Instance.EnablePlayerActions();
            GuiManager.Instance.LockMouse();
            TimeManager.Instance.TimeResume();
            Paused = false;
            pauseMenu.SetActive(false);
        }

        public void OnStack()
        {
            PlayerInstance.Instance.TeleportPlayer(new Vector3(0, 100, 0));
            HideMenu();
        }

        public void OnExit()
        {
            SceneManager.UnloadSceneAsync("GameMainScene");
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}