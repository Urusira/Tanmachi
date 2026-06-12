using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Enums;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.Items;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Quests.Orders;
using ShiroGe.Scripts.Tavern;
using ShiroGe.Scripts.UI;
using ShiroGe.Scripts.Utils;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ShiroGe.Scripts.Quests
{
    public class QuestOrderManager : MonoBehaviour
    {
        public static QuestOrderManager Instance { get; private set; }
        
        //[SerializeField] private GameObject currentQuestObj;
        [SerializeField] private GameObject tutorPanelObj;
        
        [SerializeField] public  OrdersBoard ordersBoard;
        
        [SerializeField] public int minDishInOrder;
        [SerializeField] public int maxDishInOrder;
        
        [SerializeField] public float standartOrderTimeLimitSeconds = 120f;
        
        //private CurrentQuestPanel _currentQuestPanel;
        
        private List<QuestOrderBase> _currentOrdersList = new List<QuestOrderBase>();
        
        //Словарь, где ключ - нпс-квестодатель, а значение - ещё один словарь, в котором ключ - идентификатор квеста, а значение - квест
        private Dictionary<NPCController, Dictionary<string, QuestOrderBase>> _npcQuestOrdersList = new Dictionary<NPCController, Dictionary<string, QuestOrderBase>>();

        public QuestOrderBase CurrentQuest { get; private set; } = null;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            //_currentQuestPanel = currentQuestObj.GetComponent<CurrentQuestPanel>();
        }

        public QuestOrderBase GenerateOrder(NPCController npc, string questId, int amountModifier)
        {
            int orderLen = 0;
            
            for (int i = 0; i < amountModifier; i++)
            {
                orderLen += Mathf.Max(1, Random.Range(minDishInOrder, maxDishInOrder+1));
            }

            if (orderLen <= 0) orderLen = Mathf.Max(1, Random.Range(minDishInOrder, maxDishInOrder+1));
            
            int maximumOrderPrice = npc.GetComponent<CashManager>().CashAmount;
            int currentOrderPrice = 0;
            
            List<MenuItem> validMenu = TavernMenuManager.Instance.GetRandomDishes(npc);
            
            if (validMenu.Count <= 0)
            {
                throw new WarningException("Menu haven't reputation-available items.");
            } 
            
            ItemSO[] items = new ItemSO[orderLen];
            List<ItemWithAmount> reDishesForBack = new List<ItemWithAmount>();

            for (int i = 0; i < orderLen; i++)
            {
                int attempts = 0;
                int maxAttempts = 10;
                
                while(items[i] == null && attempts < maxAttempts){
                    attempts++;
                    MenuItem randomMenuItem = validMenu[Random.Range(0, validMenu.Count)];
                    if (currentOrderPrice + randomMenuItem.price > maximumOrderPrice) continue;

                    items[i] = randomMenuItem.dish;
                    currentOrderPrice += randomMenuItem.price;
                }
                
                if (items[i] == null)
                {
                    items[i] = TavernMenuManager.Instance.GetCheapestDish(remainingCash: maximumOrderPrice-currentOrderPrice);
                }

                if (items[i] != null)
                {
                    foreach (Ingredient recipeIngredient in items[i].repice.ingredients)
                    {
                        ItemSO dishes = recipeIngredient.item;
                        if (dishes.itemType == ItemTypeEnum.DISHES)
                        {
                            ItemWithAmount item = reDishesForBack.Find(it => it.Item);
                            if (item != null)
                            {
                                reDishesForBack[reDishesForBack.IndexOf(item)].Amount += recipeIngredient.amount;
                            }
                            else
                            {
                                reDishesForBack.Add(new ItemWithAmount(recipeIngredient.item, recipeIngredient.amount));
                            }
                        }
                    }
                }
            }
            
            ItemStackList itemsStack = new ItemStackList();
            itemsStack.AddRange(items);
            
            OrderQuest newOrderQuest = new OrderQuest(
                questId,
                "Заказ",
                npc.NpcData.Name,
                $"{itemsStack}",
                items);

            newOrderQuest.SetReward(currentOrderPrice, reDishesForBack.ToHashSet(), newOrderQuest.rewardReputation*items.Length);
            return CreateNewOrder(npc, newOrderQuest);
        }

        public void CancelQuest(NPCController npc, string questId)
        {
            _npcQuestOrdersList[npc][questId].CancelQuest();
            RemoveOrder(npc, questId);
        }

        public QuestStatus QuestStatusCheck(NPCController npc, string questId)
        {
            if (!HasQuest(npc, questId)) return QuestStatus.INACTIVE;
            return _npcQuestOrdersList[npc][questId].Status;
        }
        
        public void QuestComplete(NPCController npc, string questId)
        {
            TutorialsManager.Instance.ShowTutorial("Orders3");
            
            _npcQuestOrdersList[npc][questId].CompleteQuest();
            
            RemoveOrder(npc, questId);
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
        
        public QuestOrderBase CreateNewOrder(NPCController npc, QuestOrderBase newOrderQuest)
        {
            //currentQuestObj.SetActive(true);
            
            //_currentQuestPanel.SetQuest(newOrderQuest);
            
            _currentOrdersList.Add(newOrderQuest);
            CurrentQuest = newOrderQuest;
            
            if (!_npcQuestOrdersList.ContainsKey(npc))
                _npcQuestOrdersList[npc] = new Dictionary<string, QuestOrderBase>();
            
            _npcQuestOrdersList[npc][newOrderQuest.ID] = newOrderQuest;

            newOrderQuest.OnFailed += FinalizeOrder;
            newOrderQuest.OnCompleted += FinalizeOrder;
            newOrderQuest.OnCancelled += FinalizeOrder;
            
            ordersBoard.AddOrder(newOrderQuest);

            return newOrderQuest;
        }
        
        [CanBeNull]
        public QuestOrderBase RemoveOrder(NPCController npc, string questId)
        {
            if (_npcQuestOrdersList[npc].ContainsKey(questId))
            {
                QuestOrderBase removedQuest = _npcQuestOrdersList[npc][questId];
                
                _npcQuestOrdersList[npc].Remove(questId);
                if (_npcQuestOrdersList[npc].Count == 0)
                    _npcQuestOrdersList.Remove(npc);
                
                UnsubscribeFromAllEvents(removedQuest);

                return removedQuest;
            }

            return null;
        }

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
        
        private void UnsubscribeFromAllEvents(QuestOrderBase orderQuest)
        {
            if (orderQuest == null) return;
    
            orderQuest.OnFailed -= FinalizeOrder;
            orderQuest.OnCompleted -= FinalizeOrder;
            orderQuest.OnCancelled -= FinalizeOrder;
        }

        public void FinalizeOrder(QuestOrderBase quest)
        {
            try
            {
                OrderQuest orderQuest = quest as OrderQuest;
                
                if (orderQuest == null)
                    throw new WarningException(
                        $"Не удаётся привести аргумент к типу OrderQuest или передан пустой параметр: {quest}");
                
                UnsubscribeFromAllEvents(orderQuest);

                CurrentQuest = null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void StartQuest(NPCController npc, string questId)
        {
            if (_npcQuestOrdersList[npc].ContainsKey(questId))
            {
                _npcQuestOrdersList[npc][questId].StartQuest(standartOrderTimeLimitSeconds);
                TutorialsManager.Instance.ShowTutorial("Orders");
                TutorialsManager.Instance.ShowTutorial("Orders2");
            }
            else Debug.LogWarning("You tried to staring not exist quest");
        }
    }
}