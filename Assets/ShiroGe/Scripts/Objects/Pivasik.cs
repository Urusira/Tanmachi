using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Pivasik : Interactable
    {
        public override PlayerActionsState Interact()
        {
            try
            {
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