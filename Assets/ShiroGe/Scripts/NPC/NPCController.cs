using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.Quests;
using ShiroGe.Scripts.Quests.Orders;
using ShiroGe.Scripts.Tavern;
using ShiroGe.Scripts.Utils;
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
        public event Action<NPCController> OnReadyTavernLeave;
        public event Action<NPCController> OnLeaderSayLeave;
        public event Action<NPCController> OnNpcDestroyed;
        
        [field: SerializeField] public NPCData NpcData { get; private set; }
        
        [Header("Названия диалоговых Yarn-нод")]
        [field: SerializeField] public string HungryInTavernNeutral { get; private set; }
        [field: SerializeField] public string HungryInTavernImNotALider { get; private set; }

        [field: SerializeField] public string InTavernStandingNeutral { get; private set; } = null;
        [field: SerializeField] public string DefaultNeutralDialog { get; private set; }
        
        [field: SerializeField] public bool CanNeuralTalk { get; private set; }  = true;

        public bool CanWalk => _navigator.CanWalk;
        
        [SerializeField] public bool haveHint = true;

        public int startCapital = 50;
        
        public float failedLeavingAwaitTime = 30f;
        public float preTableLeavingAwaitTime = 5f;
        public float eatingTime = 10f;
        
        
        
        public bool InTavern { get; private set; }  = false;
        public bool WaitingOrder { get; private set; }  = false;
        public bool Seating { get; private set; }  = false;
        public bool Eating { get; private set; }  = false;
        public bool Angry { get; private set; }  = false;
        public bool Funny { get; private set; }  = false;
        
        public bool WithGroup { get; private set; }  = false;
        public bool GroupLeader { get; private set; }  = false;
        
        public bool ReadyTavernLeave { get; private set; }  = false;
        
        private List<NPCController> _group = new List<NPCController>();

        public List<NPCController> Group
        {
            get
            {
                return _group;
            }
        }

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
            
            _dialog.NPCRegister(this);
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
            TavernTable table = TavernTablesManager.Instance.GetAvailableTable(1, NpcData.Introvert);
            SitPlace place = table?.GetAvailablePlace();
            if (place != null)
            {
                place.TryReservePlace(NpcData.Introvert);
                _interactor.SetTarget(place.gameObject);
                _interactor.MoveAndInteract();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool GoToTavern(TavernTable table, bool groupLeader)
        {
            SitPlace place = table?.GetAvailableAndReservePlace(NpcData.Introvert);
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

            ReadyTavernLeave = true;
            _interactor.CancelMoveAndInteract();
            LeaveTavernTable();
        }
        
        public void LeaveTavernTable()
        {
            if(_occupedTavernPlaceTable.HasValue)
            {
                if(ReadyTavernLeave)
                {
                    KeyValuePair<TavernTable, SitPlace> tablePlacePair = _occupedTavernPlaceTable.Value;
                    SitPlace sitPlace = tablePlacePair.Value;
                    TavernTable table = tablePlacePair.Key;

                    sitPlace.ReleasePlace(table.TableFullReserved);

                    _occupedTavernPlaceTable = null;

                    _seating.StandUp();

                    _navigator.LocomotionUnblock(false);

                    _navigator.GoToLastDestination();
                }
                else
                {
                    Debug.LogWarning("WTF? Your NPC trying leave table without ready flag!");
                }
            }
            else Debug.LogWarning("Your NPC trying leave table, but table null.");
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
            _seating.OnSeat -= Seated;
            _seating.OnStand -= StandUp;
            OnNpcDestroyed?.Invoke(this);
        }

        public string GetActualDialog()
        {
            if (InTavern && Seating && !Eating && (!WithGroup || (WithGroup && GroupLeader)))
            {
                return HungryInTavernNeutral;
            }
            else if (WithGroup && !GroupLeader)
            {
                return HungryInTavernImNotALider;
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
            ReadyTavernLeave = true;
            
            TimerService.Instance.AddTimer(preTableLeavingAwaitTime, LeaveTavernTable);
        }
        public void OrderFail(QuestOrderBase failedQuest)
        {
            Angry = true;
            WaitingOrder = false;
            
            failedQuest.OnFailed -= OrderFail;
            failedQuest.OnCancelled -= OrderCancel;
            
            ReadyTavernLeave = true;
            
            TimerService.Instance.AddTimer(failedLeavingAwaitTime, LeaveTavernTable);
        }
        public void OrderComplete(QuestOrderBase completedQuestBase)
        {
            if (completedQuestBase is not OrderQuest completedQuest)
                throw new WarningException(
                    $"Не удаётся привести аргумент к типу OrderQuest или передан пустой параметр: {completedQuestBase}, лидер группы");

            WaitingOrder = false;

            completedQuest.OnFailed -= OrderFail;
            completedQuest.OnCancelled -= OrderCancel;
            completedQuest.OnCompleted -= OrderComplete;
            //completedQuest.OnOrderQuestCompleted -= OrderComplete;
                
            Transaction reward = new NpcToPlayerTransaction(gameObject, GameObject.FindWithTag("Player"),
                completedQuest.rewardCash, completedQuest.rewardItems);
            
            if(!WithGroup)
            {
                StartCoroutine(Buffet(completedQuest.RequiredItems, reward));
            }
            else if (GroupLeader)
            {
                List<ItemSO> allDishes = new List<ItemSO>();
                foreach (ItemWithAmount item in completedQuest.RequiredItems)
                {
                    for (int i = 0; i < item.Amount; i++)
                    {
                        allDishes.Add(item.Item);
                    }
                }
                List<ItemSO>[] memberDishes = new List<ItemSO>[_group.Count];
                
                for (int i = 0; i < memberDishes.Length; i++)
                    memberDishes[i] = new List<ItemSO>();
    
                int memberIndex = 0;
                foreach (var dish in allDishes)
                {
                    memberDishes[memberIndex % _group.Count].Add(dish);
                    memberIndex++;
                }
    
                for (int i = 0; i < _group.Count; i++)
                {
                    NPCController member = _group[i];
                    
                    ItemStackList dishes = new ItemStackList();
                    dishes.AddRange(memberDishes[i]);
        
                    member.StartCoroutine(member.Buffet(dishes, _group[i].GroupLeader ? reward : null));
                }
            }
        }
        
        private IEnumerator Buffet(ItemStackList dishList, Transaction transaction = null)
        {
            if(Eating) yield break;
            
            Eating = true;
            if (_occupedTavernPlaceTable.HasValue)
            {
                foreach (ItemWithAmount dish in dishList)
                {
                    _occupedTavernPlaceTable.Value.Value.RemoveDish();
                    _occupedTavernPlaceTable.Value.Value.SetDish(dish.Item.itemWorldPrefab);
                    yield return new WaitForSeconds(eatingTime * dish.Amount);
                }

                _occupedTavernPlaceTable.Value.Value.RemoveDish();
            }

            Eating = false;
            Funny = true;
            
            yield return new WaitForSeconds(preTableLeavingAwaitTime);
            
            if (transaction != null && transaction.Validate()) transaction.Commit();

            ReadyTavernLeave = true;
            
            OnReadyTavernLeave?.Invoke(this);
            
            if(!WithGroup) LeaveTavernTable();
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

        public void LockMovement(Vector3 lookingTarget)
        {
            if (Seating || Eating) return;
            
            if(lookingTarget != Vector3.zero)
                _navigator.FixLookAt(lookingTarget);
            
            _navigator.LocomotionBlock();
        }
        public void UnlockMovement()
        {
            if (Seating || Eating) return;
            
            _navigator.UnfixLook();
            
            _navigator.LocomotionUnblock(true);
        }

        public void SetGroup(List<NPCController> groupList)
        {
            _group.Clear();
            _group.AddRange(groupList);
            
            WithGroup = true;
            if (_group[0] == this) GroupLeader = true;

            if (GroupLeader)
            {
                foreach (NPCController member in _group)
                {
                    member.OnReadyTavernLeave += OnReadyTavernLeaveHandler;
                }
            }
            else {
                if (_group[0].GroupLeader)
                {
                    _group[0].OnLeaderSayLeave += OnLeaderSayLeaveHandler;
                }
                else
                {
                    Debug.LogWarning("Group behaviour error: zero person in group is not a leader");
                }
            }
        }

        private void OnReadyTavernLeaveHandler(NPCController memberInvoker)
        {
            if(GroupIsReadyLeave())
            {
                foreach (NPCController member in _group)
                {
                    member.OnReadyTavernLeave -= OnReadyTavernLeaveHandler;
                }

                OnLeaderSayLeave?.Invoke(this);

                LeaveTavernTable();
            }
        }

        private bool GroupIsReadyLeave()
        {
            foreach (NPCController member in _group)
            {
                if(!member.ReadyTavernLeave) return false;
            }
            
            return true;
        }

        private void OnLeaderSayLeaveHandler(NPCController member)
        {
            _group[0].OnLeaderSayLeave -= OnLeaderSayLeaveHandler;
            LeaveTavernTable();
        }

        private void TransitCashToLeader(int amount)
        {
            if (GroupLeader) return;
            
            Transaction tr = new NpcToNpcTransaction(this.gameObject, _group[0].gameObject, amount, null);
            if(tr.Validate()) tr.Commit();
            else Debug.LogWarning("WTF? THIS NPC CANNOT GIVE TO LEADER THAT CASH!");
        }
        
        public string GenerateQuest(string questId)
        {
            if (WithGroup)
            {
                foreach (NPCController member in _group)
                {
                    Debug.LogWarning($"Trying send leader a {member._cash.CashAmount} cash");
                    member.TransitCashToLeader(member._cash.CashAmount);
                }
            }
            
            if(QuestOrderManager.Instance.GenerateOrder(this, questId, (_group.Count > 0 ? _group.Count : 1)) is OrderQuest newQuest)
            {
                newQuest.OnStarted += QuestOrderSubscribe;
                newQuest.OnDestroyed += QuestOrderUnsubscribe;
            }
            else
            {
                throw new InvalidCastException("Cannot cast [quest:" + questId + "] to " + GetType().Name);
            }

            return newQuest.RequiredItems.ToString();
        }

        public void QuestNotAccepted(string questId)
        {
            QuestOrderBase quest = QuestOrderManager.Instance.RemoveOrder(this, questId);
            if(quest != null) quest.Destroy();

            if (_group.Count != 0)
            {
                if (GroupLeader)
                {
                    OnLeaderSayLeave?.Invoke(this);
                }
                else
                { 
                    /*
                        TODO: Переработать, немного дырявое место. По идее если у нас самостоятельная группа, то пока не будет отказано всем - не уйдёт никто.
                        Однако если не давать членам группы самостоятельный диалог выдачи квеста, мы сюда и не сможем попасть никак. Пускай это место
                        останется окном для расширения системы.
                     */
                    
                    ReadyTavernLeave = true;
                    OnReadyTavernLeave?.Invoke(this);
                }
            }
            else
            {
                /*
                 * TODO: Тут можно добавить попытки перегенерировать заказ, когда станет больше блюд, чтобы НПС не сразу подрывался и убегал
                 */
                ReadyTavernLeave = true;
                LeaveTavernTable();
            }
        }

        public string GetGroupLeaderName()
        {
            return _group[0].NpcData.Name;
        }

        private void QuestOrderUnsubscribe(QuestOrderBase quest)
        {
            WaitingOrder = false;
            quest.OnStarted -= QuestOrderSubscribe;
            quest.OnFailed -= OrderFail;
            quest.OnCancelled -= OrderCancel;
            quest.OnCompleted -= OrderComplete;
        }

        public void SetRandomAvoidancyPriority()
        {
            _navigator.SetRandomAvoidancyPrio();
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}