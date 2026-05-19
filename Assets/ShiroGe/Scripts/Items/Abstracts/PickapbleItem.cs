using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public abstract class PickapbleItem : Interactable
    {
        public int amount = 1;
        
        public ItemSO scriptableItem;
        
        public override PlayerActionsState Interact()
        {
            InventoryManager.Instance.AddItem(scriptableItem, amount);
            
            /*try
            {
                InventoryManager.Instance.AddItem(scriptableItem, amount);
                GetComponentInParent<AssignDestoryer>().Destroyer();
            }
            catch (NullReferenceException _){
                Destroy(gameObject);
            }*/
            
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

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF для подбора";
        }
    }
}