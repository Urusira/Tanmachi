using System;
using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Inventory;
using UnityEngine;

namespace ShiroGe.Scripts.Quests.Orders
{
    public class TestOrder : QuestOrderBase
    {
        private readonly ItemSO _item = null;

        public TestOrder(string id, string title, string description, ItemSO itemForCheck) : base(id, title, description)
        {
            _item = itemForCheck;
        }

        public override void StartQuest(float timeLimit = 0)
        {
            
            
            base.StartQuest(timeLimit);
        }

        public override void CompleteQuest()
        {
            InventoryManager.Instance.RemoveItem(_item, 1);
            
            PlayerInstance.Instance.GiveCash(2);
            
            base.CompleteQuest();
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
            return InventoryManager.Instance.InventoryItemWithAmountCheck(_item, 1);
        }
    }
}