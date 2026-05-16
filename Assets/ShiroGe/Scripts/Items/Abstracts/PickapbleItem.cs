using System;
using ShiroGe.CharacterController;

namespace ShiroGe.Scripts.Items
{
    public abstract class PickapbleItem : Interactable
    {
        public int amount = 1;
        
        public ItemSO scriptableItem;
        
        public override PlayerActionsState Interact()
        {
            try
            {
                InventoryManager.Instance.AddItem(scriptableItem, amount);
                GetComponentInParent<AssignDestoryer>().Destroyer();
            }
            catch (NullReferenceException _){
                Destroy(gameObject);
            }

            return PlayerActionsState.PickingUp;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF для подбора";
        }
    }
}