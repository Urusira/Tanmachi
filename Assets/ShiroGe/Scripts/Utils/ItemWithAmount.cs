using System;
using UnityEngine;

namespace ShiroGe.Scripts.Quests
{
    [Serializable]
    public class ItemWithAmount
    {
        [field: SerializeField] public ItemSO Item { get; private set; }
        [field: SerializeField] public int Amount;
        
        public ItemWithAmount(ItemSO item, int amount)
        {
            this.Item = item;
            this.Amount = amount;
        }
    }
}