using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public abstract class Station : Interactable
    {
        public string stationName;
        
        public override PlayerActionsState Interact()
        {
            Debug.unityLogger.Log($"Взаимодействие с объектом {gameObject.name}");
            
            return PlayerActionsState.Default;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{stationName}\nF для взаимодействия";
        }
    }
}