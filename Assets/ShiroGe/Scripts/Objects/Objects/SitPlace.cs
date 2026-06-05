using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.NPC;
using Unity.VisualScripting;
using UnityEngine;


namespace ShiroGe.Scripts.Tavern
{
    public class SitPlace : Interactable
    {
        public event System.Action<SitPlace> OnPlaceReserved;
        public event System.Action<SitPlace> OnPlaceTaken;
        public event System.Action<SitPlace> OnPlaceVacated;

        [field: SerializeField] public bool Available { get; private set; } = true;
        
        [field: SerializeField] public GameObject PlacedEntity { get; private set; }

        private GameObject _sitPlace;

        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            throw new NotImplementedException();
        }

        protected override NPCActionsState NpcOverridableInteract(GameObject npc)
        {
            TakePlace(npc);
            
            return NPCActionsState.Sit;
        }

        private void Awake()
        {
            _sitPlace = gameObject;
            Available = true;
        }

        protected override void Initiate()
        {
            return;
        }

        public void ReservePlace()
        {
            Available = false;
            OnPlaceReserved?.Invoke(this);
        }

        public void UnreservePlace()
        {
            Available = true;
        }

        public void TakePlace(GameObject npc)
        {
            if(Available) ReservePlace();
            
            PlacedEntity = npc;
            Available = false;
            
            OnPlaceTaken?.Invoke(this);

            EntityPositionCorrect();
        }

        public GameObject ReleasePlace()
        {
            if(!Available) UnreservePlace();
            
            GameObject tempEntity = PlacedEntity;
            PlacedEntity = null;
            OnPlaceVacated?.Invoke(this);
            
            return tempEntity;
        }

        public void EntityPositionCorrect()
        {
            PlacedEntity.transform.position = new Vector3(transform.position.x, PlacedEntity.transform.position.y, transform.position.z);
            PlacedEntity.transform.rotation = transform.rotation;
        }
    }
}