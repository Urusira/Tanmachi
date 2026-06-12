using System;
using JetBrains.Annotations;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.NPC;
using UnityEngine;

namespace ShiroGe.Scripts
{
    public abstract class Interactable : MonoBehaviour
    { 
        [SerializeField] protected bool playerCanInteract = true;
        [SerializeField] protected bool npcCanInteract = false;
        
        public string name;
        
        protected Outline InteractableOutline; 
        
        private Color _outlineColor = Color.white;
        private float _outlineWidthOutline = 10f;

        public PlayerActionsState PlayerInteract(GameObject player)
        {
            if (!playerCanInteract) return PlayerActionsState.Default;
            else return PlayerOverridableInteract(player);
        }

        protected abstract PlayerActionsState PlayerOverridableInteract(GameObject player);

        public virtual NPCActionsState NpcInteract(GameObject npc)
        {
            if (!npcCanInteract) return NPCActionsState.Default;
            else return NpcOverridableInteract(npc);
        }

        protected abstract NPCActionsState NpcOverridableInteract(GameObject npc);

        private void Awake()
        {
            if (TryGetComponent<Outline>(out InteractableOutline))
            {
                InteractableOutline.enabled = false;
            }
            else
            {
                InteractableOutline = gameObject.AddComponent<Outline>();
                InteractableOutline.OutlineMode = Outline.Mode.OutlineAll;
                OutlineSetParams();
                InteractableOutline.enabled = false;
            }

            Initiate();
        }


        public void OutlineUpdate(Color outlineColor, float outlineWidthOutline)
        {
            _outlineColor = outlineColor;
            _outlineWidthOutline = outlineWidthOutline;
            OutlineSetParams();
        }

        private void OutlineSetParams()
        {
            InteractableOutline.OutlineColor = _outlineColor;
            InteractableOutline.OutlineWidth = _outlineWidthOutline;
        }

        [CanBeNull]
        public virtual String ShowHint()
        {
            if (!playerCanInteract) return null;
            InteractableOutline.enabled = true;
            return "Нажмите F для взаимодействия";
        }

        public void HideHint()
        {
            if (!playerCanInteract) return;
            InteractableOutline.enabled = false;
        }
        
        protected abstract void Initiate();

        public void BlockPlayerInteractable()
        {
            playerCanInteract = false;
        }
        
        public void UnblockPlayerInteractable()
        {
            playerCanInteract = true;
        }
    }
}