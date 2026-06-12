using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.Quests;
using ShiroGe.Scripts.Quests.Orders;
using ShiroGe.Scripts.Tavern;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn;

namespace ShiroGe.Scripts.NPC
{
    [RequireComponent(typeof(NPCDialogInteract))]
    [RequireComponent(typeof(NPCInteractor))]
    [RequireComponent(typeof(NPCNavigator))]
    [RequireComponent(typeof(NPCAnimation))]
    [RequireComponent(typeof(CashManager))]
    [RequireComponent(typeof(SeatingRig))]
    [RequireComponent(typeof(NPCState))]
    public class NPCController : MonoBehaviour
    {
        public event Action<NPCController> OnNpcDestroyed;
        
        public NPCData NpcData { get; private set; }
        
        [Header("Названия диалоговых Yarn-нод")]
        [field: SerializeField] public string HungryInTavernNeutral { get; private set; }

        [field: SerializeField] public string InTavernStandingNeutral { get; private set; } = null;
        [field: SerializeField] public string DefaultNeutralDialog { get; private set; }

        public int startCapital = 50;
        
        public float preTableLeavingAwaitTime = 5f;
        public float eatingTime = 10f;
        
        public bool InTavern { get; private set; }  = false;
        public bool WaitingOrder { get; private set; }  = false;
        public bool Seating { get; private set; }  = false;
        public bool Eating { get; private set; }  = false;

        private NPCDialogInteract _dialog;
        private NPCInteractor _interactor;
        private NPCNavigator _navigator;
        private NPCAnimation _animation;
        private NPCState _moveState;
        private SeatingRig _seating;
        private CashManager _cash;

        private KeyValuePair<TavernTable, SitPlace>? _occupedTavernPlaceTable = null;

        private void Awake()
        {
            _dialog = GetComponent<NPCDialogInteract>();
            _interactor = GetComponent<NPCInteractor>();
            _navigator = GetComponent<NPCNavigator>();
            _animation = GetComponent<NPCAnimation>();
            _moveState = GetComponent<NPCState>();
            _seating = GetComponent<SeatingRig>();
            _cash = GetComponent<CashManager>();
            
            //_dialog.NPCRegister(this);
            _interactor.NavigatorInject(_navigator);
            
            _cash.AddCash(startCapital);

            _seating.OnSeat += Seated;
            _seating.OnStand += StandUp;
        }

        public void SetNpcData(NPCData npcData)
        {
            this.NpcData = npcData;
            _dialog.NPCRegister(this);
        }

        public bool GoToTavern()
        {
            TavernTable table = TavernTablesManager.Instance.GetAvailableTable(1, NpcData.Loner);
            SitPlace place = table?.GetAvailablePlace();
            if (place != null)
            {
                place.TryReservePlace(NpcData.Loner);
                _interactor.SetTarget(place.gameObject);
                _interactor.MoveAndInteract();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool GoToTavern(TavernTable table)
        {
            SitPlace place = table?.GetAvailableAndReservePlace(NpcData.Loner);
            if (place != null)
            {
                _interactor.SetTarget(place.gameObject);
                _interactor.MoveAndInteract();

                _occupedTavernPlaceTable = new KeyValuePair<TavernTable, SitPlace>(table, place);
                return true;
            }
            else
            {
                _occupedTavernPlaceTable = null;
                return false;
            }
        }
        
        public bool GoToTavernWithGroup(TavernTable table)
        {
            SitPlace place = table?.GetAvailableAndReservePlace(false, true);
            if (place != null)
            {
                _interactor.SetTarget(place.gameObject);
                _interactor.MoveAndInteract();
                
                _occupedTavernPlaceTable = new KeyValuePair<TavernTable, SitPlace>(table, place);
                
                return true;
            }
            else
            {
                _occupedTavernPlaceTable = null;
                
                return false;
            }
        }

        public void CancelGoingToTavern()
        {
            if (InTavern) return;
            
            _interactor.CancelMoveAndInteract();
            LeaveTavernTable();
        }
        
        public void LeaveTavernTable()
        {
            if(_occupedTavernPlaceTable.HasValue)
            {
                KeyValuePair<TavernTable, SitPlace> tablePlacePair = _occupedTavernPlaceTable.Value;
                SitPlace sitPlace = tablePlacePair.Value;
                TavernTable table = tablePlacePair.Key;
                
                sitPlace.ReleasePlace(table.TableFullReserved);
                
                _occupedTavernPlaceTable = null;
                
                _seating.StandUp();

                _navigator.LocomotionUnblock();

                _navigator.GoToLastDestination();
            }
        }

        public void SetGoingTarget(Vector3 targetPos)
        {
            _navigator.MoveToTarget(targetPos);
        }
        
        public void SetBaseGoingTarget(Vector3 targetPos)
        {
            _navigator.SetBaseDestination(targetPos);
        }

        public void GoWandering()
        {
            _navigator.isWandering = true;
        }
    
        private void OnDestroy()
        {
            OnNpcDestroyed?.Invoke(this);
        }

        public string GetActualDialog()
        {
            if (InTavern && Seating && !Eating)
            {
                return HungryInTavernNeutral;
            }
/*
            if (InTavern && InTavernStandingNeutral != null)
            {
                return InTavernStandingNeutral;
            }
*/
            return DefaultNeutralDialog;
        }

        public void TavernEntry()
        {
            InTavern = true;
        }
        public void TavernLeave()
        {
            InTavern = false;
        }

        public void OrderGet()
        {
            WaitingOrder = true;
        }
        public void OrderCancel(QuestOrderBase cancelledQuest)
        {
            WaitingOrder = false;
            
            cancelledQuest.OnFailed -= OrderFail;
            cancelledQuest.OnCancelled -= OrderCancel;
            
            TimerService.Instance.AddTimer(preTableLeavingAwaitTime, LeaveTavernTable);
        }
        public void OrderFail(QuestOrderBase failedQuest)
        {
            WaitingOrder = false;
            
            failedQuest.OnFailed -= OrderFail;
            failedQuest.OnCancelled -= OrderCancel;

            TimerService.Instance.AddTimer(preTableLeavingAwaitTime, LeaveTavernTable);
        }
        public void OrderComplete(QuestOrderBase completedQuestBase)
        {
            if (completedQuestBase is not OrderQuest completedQuest)
                throw new WarningException(
                    $"Не удаётся привести аргумент к типу OrderQuest или передан пустой параметр: {completedQuestBase}");
            
            WaitingOrder = false;
            
            completedQuest.OnFailed -= OrderFail;
            completedQuest.OnCancelled -= OrderCancel;
            completedQuest.OnCompleted -= OrderComplete;
            //completedQuest.OnOrderQuestCompleted -= OrderComplete;
            
            StartCoroutine(Buffet(completedQuest));
        }

        private IEnumerator Buffet(OrderQuest completedQuest)
        {
            if(Eating) yield break;
            
            Eating = true;
            if (_occupedTavernPlaceTable.HasValue)
            {
                //TODO: Если у нпс будут инвентари, то тут надо прописать удаление наградных предметов из инвентаря НПС
                foreach (ItemWithAmount dish in completedQuest.RequiredItems)
                {
                    _occupedTavernPlaceTable.Value.Value.RemoveDish();
                    _occupedTavernPlaceTable.Value.Value.SetDish(dish.Item.itemWorldPrefab);
                    yield return new WaitForSeconds(eatingTime * dish.Amount);
                }

                _occupedTavernPlaceTable.Value.Value.RemoveDish();
            }

            Transaction reward = new NpcToPlayerTransaction(gameObject, GameObject.FindWithTag("Player"),
                completedQuest.rewardCash, completedQuest.rewardItems);

            yield return new WaitForSeconds(preTableLeavingAwaitTime);

            if (reward.Validate()) reward.Commit();

            Eating = false;

            LeaveTavernTable();
            
        }

        public void Seated()
        {
            Seating = true;
        }
        public void StandUp()
        {
            Seating = false;
        }

        public void QuestOrderSubscribe(QuestOrderBase quest)
        {
            OrderGet();
            quest.OnFailed += OrderFail;
            quest.OnCancelled += OrderCancel;
            quest.OnCompleted += OrderComplete;
        }

        public void LockMovment(Vector3 lookingTarget)
        {
            if (Seating || Eating) return;
            
            if(lookingTarget != Vector3.zero)
                _navigator.FixLookAt(lookingTarget);
            
            _navigator.LocomotionBlock();
        }
        public void UnockMovment()
        {
            if (Seating || Eating) return;
            
            _navigator.UnfixLook();
            
            _navigator.LocomotionUnblock();
        }
    }
}