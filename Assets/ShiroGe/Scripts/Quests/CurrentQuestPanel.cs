using System;
using System.Collections;
using ShiroGe.Scripts.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShiroGe.Scripts.Quests
{
    public class CurrentQuestPanel : MonoBehaviour
    {
        [SerializeField] private GameObject questTitleObj;
        [SerializeField] private GameObject questDescriptionObj;
        [SerializeField] private GameObject questTimerObj;
        [SerializeField] private GameObject questSuccessfulCompleteObj;
        [SerializeField] private GameObject questFailedObj;
        [SerializeField] private GameObject questCancelledObj;
        
        private TextMeshProUGUI _questTitleText;
        private TextMeshProUGUI _questDescriptionText;
        private TextMeshProUGUI _questTimerText;

        private void Awake()
        {
            _questTitleText =  questTitleObj.GetComponent<TextMeshProUGUI>();
            _questDescriptionText =  questDescriptionObj.GetComponent<TextMeshProUGUI>();
            _questTimerText =   questTimerObj.GetComponent<TextMeshProUGUI>();
            
            ResetQuest();
            gameObject.SetActive(false);
        }

        public void SetQuest(QuestOrderBase newQuest)
        {
            _questTitleText.text = newQuest.Title;
            _questDescriptionText.text = newQuest.Description;
        }

        public void ResetQuest()
        {
            _questTitleText.text = "";
            _questDescriptionText.text = "";
        }

        public void TimerUpdate(float remainingTime)
        {
            _questTimerText.text = remainingTime.ToString("F2");
        }

        public void SuccessfulComplete(QuestOrderBase quest)
        {
            questSuccessfulCompleteObj.SetActive(true);
            StartCoroutine(QuestFinalize(questSuccessfulCompleteObj));
        }

        public void Failed(QuestOrderBase _)
        {
            questFailedObj.SetActive(true);
            StartCoroutine(QuestFinalize(questFailedObj));
        }

        public void Cancelled(QuestOrderBase _)
        {
            questCancelledObj.SetActive(true);
            StartCoroutine(QuestFinalize(questCancelledObj));
        }

        private IEnumerator QuestFinalize(GameObject finalizePanel)
        {
            ResetQuest();
            yield return new WaitForSecondsRealtime(4f);
            finalizePanel.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}