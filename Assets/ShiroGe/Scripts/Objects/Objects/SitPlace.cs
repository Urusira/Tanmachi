using System;
using System.Threading;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Items;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.World;
using Unity.VisualScripting;
using UnityEngine;


namespace ShiroGe.Scripts.Tavern
{
    public class SitPlace : Interactable
    {
        public event System.Action<SitPlace, bool> OnPlaceReserved;
        public event System.Action<SitPlace> OnPlaceTaken;
        public event System.Action<SitPlace, bool> OnPlaceVacated;

        private int _available = 1;
        
        public bool Available => _available == 1;
        
        [field: SerializeField] public GameObject PlacedEntity { get; private set; }
        [field: SerializeField] public Vector3 SittingOffset { get; private set; }

        [SerializeField] private GameObject leftLegAnchor;
        [SerializeField] private GameObject rightLegAnchor;
        
        [SerializeField] private GameObject foodAnchor;

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
            _available = 1;
        }

        private void FixedUpdate()
        {
            if(PlacedEntity != null)
                EntityPositionCorrect();
        }

        protected override void Initiate()
        {
            return;
        }

        public bool TryReservePlace(bool reserveFullTable)
        {
            int original = Interlocked.CompareExchange(ref _available, 0, 1);
            
            if(original == 1)
                OnPlaceReserved?.Invoke(this, reserveFullTable);
            
            return original == 1;
        }

        public void UnreservePlace()
        {
            Interlocked.Exchange(ref _available, 1);
        }

        public void TakePlace(GameObject entity)
        {
            if(Available) TryReservePlace(false);
            
            PlacedEntity = entity;
            
            OnPlaceTaken?.Invoke(this);

            SeatingRig rig;
            PlacedEntity.TryGetComponent(out rig);
            if (rig != null)
            {
                rig.SetLegsAnchors(leftLegAnchor.transform, rightLegAnchor.transform);
                rig.SitDown();
            }
        }

        public void ReleasePlace(bool wasReserveFullTable)
        {
            if(!Available) UnreservePlace();
            
            GameObject tempEntity = PlacedEntity;
            PlacedEntity = null;
            OnPlaceVacated?.Invoke(this, wasReserveFullTable);
        }

        public void EntityPositionCorrect()
        {
            PlacedEntity.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z)+transform.rotation*SittingOffset;
            PlacedEntity.transform.rotation = transform.rotation;
        }

        public void SetDish(GameObject obj)
        {
            Instantiate(obj, foodAnchor.transform).layer = 0;
        }

        public void RemoveDish()
        {
            if(foodAnchor.transform.childCount > 0)
                Destroy(foodAnchor.transform.GetChild(0).gameObject);
        }
    }
}