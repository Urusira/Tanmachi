using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Enums;
using ShiroGe.Scripts.NPC;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public class Furnace : Station
    {
        [SerializeField] public CraftStations typeStation = CraftStations.Furnace;
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            base.PlayerOverridableInteract(player);
            Debug.unityLogger.Log("Я почему такой вредный был... Потому что у меня велосипеда не было!");
            
            return PlayerActionsState.Default;
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            throw new NotImplementedException();
        }

        protected override void Initiate()
        {
            return;
        }
    }
}