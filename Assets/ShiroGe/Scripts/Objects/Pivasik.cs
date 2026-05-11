using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Pivasik : Interactable
    {
        public override PlayerActionsState Interact()
        {
            Destroy(gameObject);
            
            return PlayerActionsState.PickingUp;
        }
    }
}