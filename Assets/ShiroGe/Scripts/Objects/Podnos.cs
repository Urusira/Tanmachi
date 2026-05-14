using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Podnos : Interactable
    {
        public override PlayerActionsState Interact()
        {
            Destroy(gameObject);
            
            //return PlayerActionsState.PickingUp;
            return PlayerActionsState.Handling1HHorizontal;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF чтобы взять в руки";
        }
    }
}