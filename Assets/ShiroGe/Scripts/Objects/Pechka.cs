using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Pechka : Interactable
    {
        public override PlayerActionsState Interact()
        {
            Debug.unityLogger.Log("Я почему такой вредный был... Потому что у меня велосипеда не было!");
            
            return PlayerActionsState.Default;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{gameObject.name}\nF для взаимодействия";
        }
    }
}