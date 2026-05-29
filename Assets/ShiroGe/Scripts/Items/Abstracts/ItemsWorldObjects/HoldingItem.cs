using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public abstract class HoldingItem : PickupbleItem
    {
        public override PlayerActionsState Interact()
        {
            InventoryManager.Instance.AddItem(scriptableItem, amount);
            
            Destroy(gameObject);
            
            return PlayerActionsState.Handling1HHorizontal;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF чтобы взять в руки";
        }
    }
}