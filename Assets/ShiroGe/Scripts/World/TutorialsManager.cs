using System;
using ShiroGe.Scripts.UI;
using UnityEngine;

namespace ShiroGe.Scripts.World
{
    public class TutorialsManager : MonoBehaviour
    {
        [SerializeField] private TutorialPanel tutorialPanel;
        
        public static TutorialsManager Instance { get; private set; }
        
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
            tutorialPanel.ShowTutorial("Movement");
            tutorialPanel.ShowTutorial("Actions");
        }

        public void ShowTutorial(string tutorialName)
        {
            tutorialPanel.ShowTutorial(tutorialName);
        }
    }
}