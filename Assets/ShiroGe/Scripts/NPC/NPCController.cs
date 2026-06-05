using System;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.Tavern;
using UnityEngine;

namespace ShiroGe.Scripts.NPC
{
    [RequireComponent(typeof(NPCDialogInteract))]
    [RequireComponent(typeof(NPCInteractor))]
    [RequireComponent(typeof(NPCNavigator))]
    [RequireComponent(typeof(NPCAnimation))]
    [RequireComponent(typeof(CashManager))]
    [RequireComponent(typeof(NPCState))]
    public class NPCController : MonoBehaviour
    {
        public NPCData npcData { get; private set; } = new NPCData("", "NPC", "", 1, true);

        public bool GoingToTavern = true;
        
        private NPCDialogInteract _dialog;
        private NPCInteractor _interactor;
        private NPCNavigator _navigator;
        private NPCAnimation _animation;
        private NPCState _moveState;
        private CashManager _cash;

        private void Awake()
        {
            _dialog = GetComponent<NPCDialogInteract>();
            _interactor = GetComponent<NPCInteractor>();
            _navigator = GetComponent<NPCNavigator>();
            _animation = GetComponent<NPCAnimation>();
            _moveState = GetComponent<NPCState>();
            _cash = GetComponent<CashManager>();

            if(npcData != null)  npcData.SetId(GetInstanceID().ToString());
            
            _dialog.NPCRegister(this);
            _interactor.NavigatorInject(_navigator);
            
            _cash.addCash(100f);
        }

        private void Start()
        {
            if (GoingToTavern)
            {
                GoToTavern();
            }
        }

        public void SetNpcData(NPCData npcData)
        {
            this.npcData = npcData;
        }

        public void GoToTavern()
        {
            TavernTable table = TavernTablesManager.Instance.GetAvailablePlace(1, npcData.Loner);
            SitPlace place = table?.GetAvailablePlace();
            if (place != null)
            {
                place.ReservePlace();
                _interactor.SetTarget(place.gameObject);
                _interactor.MoveAndInteract();
            }
        }
    }
}