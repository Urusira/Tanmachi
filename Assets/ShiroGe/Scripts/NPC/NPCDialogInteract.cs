using System;
using ShiroGe.CharacterController;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShiroGe.Scripts.NPC
{
    public class NPCDialogInteract : Interactable
    {
        private NPCData npcData;
        
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            DialogManager.Instance.StartDialog(npcData);

            return PlayerActionsState.Default;
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            throw new NotImplementedException();
        }

        public override String ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF для разговора";
        }

        protected override void Initiate() { return; }

        public void NPCRegister(NPCController npc)
        {
            this.npcData = npc.npcData;
        }
    }
}