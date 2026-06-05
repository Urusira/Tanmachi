using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.NPC;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Cauldron : Station
    {
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            base.PlayerOverridableInteract(player);
            Debug.unityLogger.Log("О ВЕЛИКИЙ СУП НАВАРИЛИ О ПРЕКРАСНЫЙ СУП НАВАРИЛИ О ШИКАРНЫЙ СУП НАВАРИЛИ О ВЕЛИКИЙ СУП ЕШЬ СУП");

            return PlayerActionsState.Default;
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            throw new NotImplementedException();
        }

        protected override void Initiate()
        {
            this.stationName = "Котёл";
        }
    }
}