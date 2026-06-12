using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Utils;
using ShiroGe.Scripts.World;
using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public class PickupbleObject : Interactable
    {
        public int amount = 1;
        
        public ItemSO scriptableItem;
        
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            if(InventoryManager.Instance.InBuildingMode && playerCanInteract)
            {
                InventoryManager.Instance.AddItem(scriptableItem, amount);

                /*LODGroup lodGroup = GetComponentInParent<LODGroup>();
                if (lodGroup != null && LayerManager.Instance.InteractiveLayerMask.Contains(lodGroup.gameObject.layer))
                {
                    Destroy(lodGroup.gameObject);
                }
                else
                {*/
                    Destroy(gameObject);
                //}

                return PlayerActionsState.PickingUp;
            }
            
            return PlayerActionsState.Default;
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            throw new NotImplementedException();
        }

        public override string ShowHint()
        {
            if (!playerCanInteract) return null;
            
            if(InventoryManager.Instance.InBuildingMode)
            {
                base.ShowHint();
                return $"{name}\nНажмите F для подбора";
            }

            return null;
        }

        protected override void Initiate()
        {
            playerCanInteract = false;
            TimerService.Instance.AddTimer(1f, LateEnabler);
            
            name = scriptableItem.itemName;
            return;
        }

        private void LateEnabler()
        {
            playerCanInteract = true;
        }
    }
}