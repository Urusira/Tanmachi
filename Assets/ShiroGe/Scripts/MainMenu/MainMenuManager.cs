using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuDocument;
    
    private Button _playButton;
    private Button _settingsButton;
    private Button _exitButton;

    private void Awake()
    {
        VisualElement root = mainMenuDocument.rootVisualElement;
        
        _playButton = root.Q<Button>("PlayButton");
        _settingsButton = root.Q<Button>("SettingsButton");
        _exitButton = root.Q<Button>("ExitButton");

        _playButton.clickable.clicked += StartGame;
        _settingsButton.clickable.clicked += ShowSettingsMenu;
        _exitButton.clickable.clicked += ExitGame;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameMainScene");
    }
    
    private void ShowSettingsMenu()
    {
        print("Showing settings menu");
    }
    
    private void ExitGame()
    {
        Application.Quit();
    }
}
