using System;
using ShiroGe.CharacterController;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShiroGe.Scripts.NPC
{
    public class Dialog : Interactable
    {
        [SerializeField] private new string name = String.Empty;

        public override PlayerActionsState Interact()
        {
            DialogManager.Instance.StartDialog(name, GetInstanceID().ToString());

            return PlayerActionsState.Default;
        }

        public override String ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF для разговора";
        }

        public override void Initiate() { return; }
    }
}