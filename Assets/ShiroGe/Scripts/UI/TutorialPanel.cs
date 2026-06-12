using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ShiroGe.Scripts.World;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShiroGe.Scripts.UI
{
    public class TutorialPanel : MonoBehaviour
    {
        [SerializeField] private TutorialsDatabase tutorialsDatabase;
        
        [SerializeField] private GameObject tutorialPanelObj;
        [SerializeField] private GameObject tutorialPanelTitle;
        [SerializeField] private GameObject tutorialPanelDescription;
        
        [SerializeField] private bool resetTutorialsOnRestart = true;
        
        [SerializeField] private float showingDuration = 10f;
        
        private TextMeshProUGUI _tutorialTitleText;
        private TextMeshProUGUI _tutorialDescText;
        
        private RectTransform _tutorialPanelRect;
        
        private readonly List<TutorialDatabaseRow> _tutorialRowsQueue = new List<TutorialDatabaseRow>();
        
        private bool _showedPanel = false;
        
        private void Awake()
        {;
            _tutorialTitleText = tutorialPanelTitle.GetComponent<TextMeshProUGUI>();
            _tutorialDescText = tutorialPanelDescription.GetComponent<TextMeshProUGUI>();
            
            _tutorialPanelRect = tutorialPanelObj.GetComponent<RectTransform>();

            if (resetTutorialsOnRestart)
            {
                foreach (TutorialDatabaseRow row in tutorialsDatabase.tutorialDatabase)
                {
                    row.isShowed = false;
                }
            }
        }

        public void ShowTutorial(string tutorialKey)
        {
            TutorialDatabaseRow tutorialRow = tutorialsDatabase.tutorialDatabase.Find(x => x.key == tutorialKey);
            
            if (_showedPanel)
            {
                _tutorialRowsQueue.Add(tutorialRow);
            }
            else
            {
                ShowTutorialPanel(tutorialRow);
            }
        }

        private void ShowTutorialPanel(TutorialDatabaseRow tutor)
        {
            if (tutor.isShowed || _showedPanel) return;
            
            _showedPanel = true;
            _tutorialTitleText.text = tutor.title;
            _tutorialDescText.text = tutor.value;
            _tutorialPanelRect.DOLocalMoveX(-_tutorialPanelRect.rect.width*2, 0.5f).SetEase(Ease.InBounce);
            TimerService.Instance.AddTimer(showingDuration, HideTutorialPanel);
            tutor.isShowed = true;
        }

        private void HideTutorialPanel()
        {
            _tutorialPanelRect.DOLocalMoveX(-_tutorialPanelRect.rect.width, 0.5f).SetEase(Ease.InBounce);
            _tutorialTitleText.text = "";
            _tutorialDescText.text = "";
            _showedPanel = false;

            TutorialDatabaseRow nextTutorialRow = _tutorialRowsQueue.FirstOrDefault();
            if (nextTutorialRow != null)
            {
                TimerService.Instance.AddTimerWithContext(1f, ShowTutorialPanel, nextTutorialRow);
                _tutorialRowsQueue.Remove(nextTutorialRow);
            }
        }
    }
}