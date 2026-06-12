using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Quests;
using UnityEngine;

namespace ShiroGe.Scripts
{
    public class NpcToNpcTransaction : Transaction
    {
        private GameObject _fromNpc;
        private GameObject _toNpc;

        public bool Commited { get; private set; } = false;
    
        public NpcToNpcTransaction(GameObject fromNpc, GameObject toNpc, int cash, HashSet<ItemWithAmount> items)
        {
            _fromNpc = fromNpc;
            _toNpc = toNpc;
            Cash = cash;
            Items = items;
        }
    
        public override bool Validate()
        {
            return _fromNpc.GetComponent<CashManager>().CanRemoveCash(Cash) /*&& _fromNpc.Inventory.HasItems(Items)*/;
        }
    
        public override void Commit()
        {
            _fromNpc.GetComponent<CashManager>().RemoveCash(Cash);
            _toNpc.GetComponent<CashManager>().AddCash(Cash);
        
            /*foreach (ItemWithAmount item in Items)
            {
                //_fromNpc.Inventory.RemoveItem(item.Item, item.Amount);
                //_toNpc.InventoryManager.Instance.AddItem(item.Item, item.Amount);
            }*/
            
            Commited = true;
        }
    
        public override void Rollback()
        {
            _fromNpc.GetComponent<CashManager>().AddCash(Cash);
            _toNpc.GetComponent<CashManager>().RemoveCash(Cash);
            
            foreach (ItemWithAmount item in Items)
            {
                //_npc.Inventory.RemoveItem(item.Item, item.Amount);
                InventoryManager.Instance.RemoveItem(item.Item, item.Amount);
            }
            
            Commited = false;
        }
    }
}