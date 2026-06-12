using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShiroGe.Scripts.Quests;

namespace ShiroGe.Scripts.Utils
{
    [Serializable]
    public class ItemStackList : IEnumerable<ItemWithAmount>
    {
        public List<ItemWithAmount> Items = new List<ItemWithAmount>();
        
        public ItemWithAmount this[int index] => Items[index];

        public int Count => Items.Count;

        public bool Contains(ItemSO item) => Items.Any(i => i.Item == item);

        public void Clear() => Items.Clear();
        
        public int GetAmount(ItemSO item)
        {
            var existing = Items.Find(i => i.Item == item);
            return existing?.Amount ?? 0;
        }
        
        public IReadOnlyList<ItemWithAmount> GetAll() => Items.AsReadOnly();
        
        public void Add(ItemSO item, int amount)
        {
            var existing = Items.FirstOrDefault(i => i.Item == item);
            if (existing != null)
            {
                existing.Amount += amount;
            }
            else
            {
                Items.Add(new ItemWithAmount(item, amount));
            }
        }
        
        public void AddRange(IEnumerable<ItemWithAmount> items)
        {
            foreach (var item in items)
            {
                Add(item.Item, item.Amount);
            }
        }
        
        public void AddRange(IEnumerable<ItemSO> items)
        {
            foreach (var item in items)
            {
                Add(item, 1);
            }
        }
        
        public void Remove(ItemSO item, int amount)
        {
            var existing = Items.Find(i => i.Item == item);
            if (existing == null) return;
        
            if (existing.Amount <= amount)
            {
                Items.Remove(existing);
            }
            else
            {
                existing.Amount -= amount;
            }
        }
    
        public IEnumerator<ItemWithAmount> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            string result = "";

            foreach (ItemWithAmount item in this)
            {
                if (item == this[Count - 1])
                {
                    result += $"{item.Amount} {item.Item.itemName}.";
                }
                else
                {
                    result += $"{item.Amount} {item.Item.itemName}, ";
                }
            }
            
            return result;
        }
    }
}