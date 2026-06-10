using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Quests;
using UnityEngine;

namespace ShiroGe.Scripts
{
    public class NpcToPlayerTransaction : Transaction
    {
        private GameObject _npc;
        private GameObject _player;

        public bool Commited { get; private set; } = false;
    
        public NpcToPlayerTransaction(GameObject npc, GameObject player, int cash, HashSet<ItemWithAmount> items)
        {
            _npc = npc;
            _player = player;
            Cash = cash;
            Items = items;
        }
    
        public override bool Validate()
        {
            return _npc.GetComponent<CashManager>().CanRemoveCash(Cash) /*&& _npc.Inventory.HasItems(Items)*/;
        }
    
        public override void Commit()
        {
            _npc.GetComponent<CashManager>().RemoveCash(Cash);
            _player.GetComponent<CashManager>().AddCash(Cash);
        
            foreach (ItemWithAmount item in Items)
            {
                //_npc.Inventory.RemoveItem(item.Item, item.Amount);
                InventoryManager.Instance.AddItem(item.Item, item.Amount);
            }
            
            Commited = true;
        }
    
        public override void Rollback()
        {
            _npc.GetComponent<CashManager>().AddCash(Cash);
            _player.GetComponent<CashManager>().RemoveCash(Cash);
            
            foreach (ItemWithAmount item in Items)
            {
                //_npc.Inventory.RemoveItem(item.Item, item.Amount);
                InventoryManager.Instance.RemoveItem(item.Item, item.Amount);
            }
            
            Commited = false;
        }
    }
}