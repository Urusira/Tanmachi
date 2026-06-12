using System;
using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace ShiroGe.Scripts.Quests.Orders
{
    public class OrderQuest : QuestOrderBase
    {
        //private event System.Action<OrderQuest> OnOrderQuestCompleted;
        
        public readonly ItemStackList RequiredItems = new ItemStackList();

        public OrderQuest(string id, string title, string authorName, string description, ItemSO[] requiredItems) : base(id, title, authorName, description)
        {
            RequiredItems.AddRange(requiredItems);
        }

        public override void StartQuest(float timeLimit = 0)
        {
            base.StartQuest(timeLimit);
        }

        public override void CompleteQuest()
        {
            foreach (ItemWithAmount item in RequiredItems)
            {
                InventoryManager.Instance.RemoveItem(item.Item, item.Amount);
            }
            
            base.CompleteQuest();
            
            //OnOrderQuestCompleted?.Invoke(this);
        }

        public override void FailQuest()
        {
            base.FailQuest();
        }

        public override void CancelQuest()
        {
            base.CancelQuest();
        }

        public override bool ConditionCheck()
        {
            bool successful = true;
            
            foreach (ItemWithAmount item in RequiredItems)
            {
                if (!InventoryManager.Instance.InventoryItemWithAmountCheck(item.Item, item.Amount))
                {
                    successful = false;
                    break;
                }
            }

            return successful;
        }
    }
}