using System;
using System.Collections;
using System.ComponentModel;
using ShiroGe.Scripts.Tavern;
using UnityEngine;

namespace ShiroGe.Scripts.NPC
{
    public class NPCInteractor : MonoBehaviour
    {
        public Interactable InteractTarget { get; private set; }
        
        private NPCNavigator _navigator;

        public void SetTarget(GameObject target)
        {
            InteractTarget = target.GetComponent<Interactable>();
            
            if (InteractTarget == null)
            {
                throw new WarningException("NPCInteractor.SetTarget(): InteractTarget is null");
            }
        }
        
        public void MoveAndInteract()
        {
            if (InteractTarget != null)
            {
                if (_navigator != null)
                {
                    if (_navigator._lastDestination == Vector3.zero)
                    {
                        _navigator.SetLastDestinationPoint(_navigator._baseDestination);
                    }
                    _navigator.isWandering = false;
                    _navigator.MoveToTarget(InteractTarget.transform.position);
                    _navigator.OnDestinationReached += InteractWithTarget;
                }
            }
        }
        
        public void CancelMoveAndInteract()
        {
            if (_navigator != null)
            {
                _navigator.OnDestinationReached -= InteractWithTarget;
            }
        }

        private void InteractWithTarget()
        {
            _navigator.OnDestinationReached -= InteractWithTarget;
            _navigator.ResetCurrentDestination();
            _navigator.LocomotionBlock();
            
            InteractTarget.NpcInteract(gameObject);
        }

        public void NavigatorInject(NPCNavigator navigator)
        {
            _navigator = navigator;
        }
    }
}