using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.NPC;
using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public class PickupbleItem : Interactable
    {
        public int amount = 1;
        
        public ItemSO scriptableItem;
        
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            InventoryManager.Instance.AddItem(scriptableItem, amount);
            
            LODGroup lodGroup = GetComponentInParent<LODGroup>();
            if (lodGroup != null)
            {
                Destroy(lodGroup.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            return PlayerActionsState.PickingUp;
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            throw new NotImplementedException();
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF для подбора";
        }

        protected override void Initiate()
        {
            return;
        }
    }
}