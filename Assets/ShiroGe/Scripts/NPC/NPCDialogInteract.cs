using System;
using ShiroGe.CharacterController;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShiroGe.Scripts.NPC
{
    public class NPCDialogInteract : Interactable
    {
        private NPCController npc;
        
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            DialogManager.Instance.StartDialog(npc);

            return PlayerActionsState.Default;
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            throw new NotImplementedException();
        }

        public override String ShowHint()
        {
            if(npc.haveHint)
            {
                base.ShowHint();
                return $"{npc.NpcData.Name}\nНажмите F чтобы начать разговор";
            }

            return "???";
        }

        protected override void Initiate() { return; }

        public void NPCRegister(NPCController npc)
        {
            this.npc = npc;
        }
    }
}