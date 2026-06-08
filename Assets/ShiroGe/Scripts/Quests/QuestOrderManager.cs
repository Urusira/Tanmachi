using System;
using System.Collections.Generic;
using System.ComponentModel;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Quests.Orders;
using ShiroGe.Scripts.World;
using UnityEngine;

namespace ShiroGe.Scripts.Quests
{
    public class QuestOrderManager : MonoBehaviour
    {
        public static QuestOrderManager Instance { get; private set; }
        
        [SerializeField] private GameObject currentQuestObj;
        [SerializeField] private ItemSO itemForTest;
        
        private CurrentQuestPanel _currentQuestPanel;
        
        private List<TestOrder> _currentOrdersList = new List<TestOrder>();
        
        private Dictionary<NPCController, Dictionary<string, QuestOrderBase>> _npcQuestOrdersList = new Dictionary<NPCController, Dictionary<string, QuestOrderBase>>();

        public TestOrder CurrentQuest { get; private set; } = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            _currentQuestPanel = currentQuestObj.GetComponent<CurrentQuestPanel>();
        }

        public void CreateTestOrder(NPCController npc, string questId)
        {
            TestOrder newOrder = new TestOrder(
                questId,
                "СКАЗАНИЕ О ВЕЛИКОМ СУПЕ",
                "Приготовьте и отдайте NPC грибное варево (Готовится в котле).",
                itemForTest);
            
            CreateNewOrder(npc, newOrder);
        }

        public void CancelQuest(NPCController npc, string questId)
        {
            _npcQuestOrdersList[npc][questId].CancelQuest();
            _npcQuestOrdersList[npc].Remove(questId);
            if (_npcQuestOrdersList[npc].Count == 0)
                _npcQuestOrdersList.Remove(npc);
        }

        public QuestStatus QuestStatusCheck(NPCController npc, string questId)
        {
            if (!HasQuest(npc, questId)) return QuestStatus.INACTIVE;
            return _npcQuestOrdersList[npc][questId].Status;
        }
        
        public void QuestComplete(NPCController npc, string questId)
        {
            _npcQuestOrdersList[npc][questId].CompleteQuest();
            
            _npcQuestOrdersList[npc].Remove(questId);
            if (_npcQuestOrdersList[npc].Count == 0)
                _npcQuestOrdersList.Remove(npc);
        }

        public QuestOrderBase GetQuest(NPCController npc, string questId)
        {
            return _npcQuestOrdersList[npc][questId];
        }
        
        public bool HasQuest(NPCController npc, string questId)
        {
            return _npcQuestOrdersList.ContainsKey(npc) && _npcQuestOrdersList[npc].ContainsKey(questId);
        }
        
        public bool QuestCompleteConditionCheck(NPCController npc, string questId)
        {
            return _npcQuestOrdersList[npc][questId].ConditionCheck();
        }
        
        public void CreateNewOrder(NPCController npc, TestOrder newOrder)
        {
            currentQuestObj.SetActive(true);
            
            _currentQuestPanel.SetQuest(newOrder);
            _currentOrdersList.Add(newOrder);
            CurrentQuest = newOrder;
            
            if (!_npcQuestOrdersList.ContainsKey(npc))
                _npcQuestOrdersList[npc] = new Dictionary<string, QuestOrderBase>();
            
            _npcQuestOrdersList[npc][newOrder.ID] = newOrder;
            
            newOrder.StartQuest(3000f);
            
            TimeManager.Instance.OnTimeTick -= CurrentQuestTimerUpdate;
            TimeManager.Instance.OnTimeTick += CurrentQuestTimerUpdate;

            newOrder.OnFailed += FinalizeOrder;
            newOrder.OnFailed += _currentQuestPanel.Failed;
            
            newOrder.OnCompleted += FinalizeOrder;
            newOrder.OnCompleted += _currentQuestPanel.SuccessfulComplete;
            
            newOrder.OnCancelled += FinalizeOrder;
            newOrder.OnCancelled += _currentQuestPanel.Cancelled;
            
            npc.QuestSubscribe(newOrder);
        }
/*
        public void NextQuest()
        {
            OrderListCleaner();
            
            TestOrder order = _currentOrdersList[_currentOrdersList.Count-1];
            if(order.Status == QuestStatus.ACTIVE)
            {
                _currentQuestPanel.SetQuest(order);
                CurrentQuest = order;

                TimeManager.Instance.OnTimeTick -= CurrentQuestTimerUpdate;
                TimeManager.Instance.OnTimeTick += CurrentQuestTimerUpdate;
            }
        }
*/
        private void OrderListCleaner()
        {
            for (int i = _currentOrdersList.Count-1; i < _currentOrdersList.Count; i--)
            {
                if (_currentOrdersList[i].Status != QuestStatus.ACTIVE)
                {
                    UnsubscribeFromAllEvents(_currentOrdersList[i]);
                    _currentOrdersList.RemoveAt(i);
                }
            }
        }
        
        private void UnsubscribeFromAllEvents(TestOrder order)
        {
            if (order == null) return;
    
            order.OnFailed -= FinalizeOrder;
            order.OnFailed -= _currentQuestPanel.Failed;
            order.OnCompleted -= FinalizeOrder;
            order.OnCompleted -= _currentQuestPanel.SuccessfulComplete;
            order.OnCancelled -= FinalizeOrder;
            order.OnCancelled -= _currentQuestPanel.Cancelled;
        }

        public void CurrentQuestTimerUpdate(float _)
        {
            if (CurrentQuest == null) return;
            
            float remainingTime = CurrentQuest.TimeLimit - CurrentQuest.Timer;
            
            _currentQuestPanel.TimerUpdate(remainingTime);
        }

        public void FinalizeOrder(QuestOrderBase quest)
        {
            try
            {
                TestOrder order = quest as TestOrder;
                
                if (order == null)
                    throw new WarningException(
                        $"Не удаётся привести аргумент к типу TestOrder или передан пустой параметр: {quest}");
                
                TimeManager.Instance.OnTimeTick -= CurrentQuestTimerUpdate;
                
                UnsubscribeFromAllEvents(order);

                _currentOrdersList.Remove(order);
                CurrentQuest = null;
                
                //if(_currentOrdersList.Count > 0) NextQuest();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            if(quest.rewardItems != null)
            {
                foreach (KeyValuePair<ItemSO, int> rewardItem in quest.rewardItems)
                {
                    InventoryManager.Instance.AddItem(rewardItem.Key, rewardItem.Value);
                }
            }

            if (quest.rewardCash > 0)
            {
                PlayerInstance.Instance.GiveCash(quest.rewardCash);
            }
        }
    }
}