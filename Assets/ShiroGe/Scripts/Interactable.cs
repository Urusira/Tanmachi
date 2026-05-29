using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts
{
    public abstract class Interactable : MonoBehaviour
    { 
        protected Outline InteractableOutline; 
        
        private Color _outlineColor = Color.white;
        private float _outlineWidthOutline = 10f;
        
        public abstract PlayerActionsState Interact();

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

        public virtual String ShowHint()
        {
            InteractableOutline.enabled = true;
            return "Нажмите для взаимодействия";
        }

        public void HideHint()
        {
            InteractableOutline.enabled = false;
        }
        
        protected abstract void Initiate();
    }
}