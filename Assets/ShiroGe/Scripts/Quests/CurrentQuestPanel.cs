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
        
        [SerializeField] private bool dieAfterFinalize;
        
        private TextMeshProUGUI _questTitleText;
        private TextMeshProUGUI _questDescriptionText;
        private TextMeshProUGUI _questTimerText;

        private void Awake()
        {
            _questTitleText =  questTitleObj.GetComponent<TextMeshProUGUI>();
            _questDescriptionText =  questDescriptionObj.GetComponent<TextMeshProUGUI>();
            _questTimerText =   questTimerObj.GetComponent<TextMeshProUGUI>();
            
            if(!dieAfterFinalize)
            {
                _questTitleText.text = "";
                _questDescriptionText.text = "";

                gameObject.SetActive(false);
            }
        }

        public void SetQuest(QuestOrderBase newQuest)
        {
            _questTitleText.text = newQuest.Title;
            _questDescriptionText.text = newQuest.Description;
            
            newQuest.OnCompleted += Completed;
            newQuest.OnCancelled += Cancelled;
            newQuest.OnFailed += Failed;
            newQuest.OnRemainingTimeChanged += TimerUpdate;
            
        }

        public void ResetQuest(QuestOrderBase quest)
        {
            _questTitleText.text = "";
            _questDescriptionText.text = "";
            
            quest.OnCompleted -= Completed;
            quest.OnCancelled -= Cancelled;
            quest.OnFailed -= Failed;
            quest.OnRemainingTimeChanged -= TimerUpdate;
        }

        public void TimerUpdate(float remainingTime)
        {
            _questTimerText.text = (remainingTime/60).ToString("F2");
        }

        public void Completed(QuestOrderBase quest)
        {
            questSuccessfulCompleteObj.SetActive(true);
            StartCoroutine(QuestFinalize(questSuccessfulCompleteObj, quest));
        }

        public void Failed(QuestOrderBase quest)
        {
            questFailedObj.SetActive(true);
            StartCoroutine(QuestFinalize(questFailedObj, quest));
        }

        public void Cancelled(QuestOrderBase quest)
        {
            questCancelledObj.SetActive(true);
            StartCoroutine(QuestFinalize(questCancelledObj, quest));
        }

        private IEnumerator QuestFinalize(GameObject finalizePanel, QuestOrderBase quest)
        {
            ResetQuest(quest);
            yield return new WaitForSecondsRealtime(4f);
            finalizePanel.SetActive(false);
            
            if(!dieAfterFinalize) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }
}