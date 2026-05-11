using System;
using ShiroGe.CharacterController;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Kotelok : Interactable
    {
        public override PlayerActionsState Interact()
        {
            Debug.unityLogger.Log("О ВЕЛИКИЙ СУП НАВАРИЛИ О ПРЕКРАСНЫЙ СУП НАВАРИЛИ О ШИКАРНЫЙ СУП НАВАРИЛИ О ВЕЛИКИЙ СУП ЕШЬ СУП");

            return PlayerActionsState.Default;
        }
    }
}