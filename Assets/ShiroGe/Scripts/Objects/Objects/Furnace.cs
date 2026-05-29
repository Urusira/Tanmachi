using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Furnace : Station
    {
        public override PlayerActionsState Interact()
        {
            base.Interact();
            Debug.unityLogger.Log("Я почему такой вредный был... Потому что у меня велосипеда не было!");
            
            return PlayerActionsState.Default;
        }
        
        protected override void Initiate()
        {
            this.stationName = "Печка с плитой";
        }
    }
}