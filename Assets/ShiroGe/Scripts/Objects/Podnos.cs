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
    }
}