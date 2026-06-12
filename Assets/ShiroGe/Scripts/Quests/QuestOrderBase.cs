using System;
using System.Collections.Generic;
using System.ComponentModel;
using ShiroGe.Scripts.Quests.Orders;
using ShiroGe.Scripts.Tavern;
using ShiroGe.Scripts.World;
using Unity.VisualScripting;
using UnityEngine;

namespace ShiroGe.Scripts.Quests
{
    public abstract class QuestOrderBase
    {
        public event System.Action<QuestOrderBase> OnStarted;
        public event System.Action<QuestOrderBase> OnFailed;
        public event System.Action<QuestOrderBase> OnCompleted;
        public event System.Action<QuestOrderBase> OnCancelled;
        public event System.Action<float> OnRemainingTimeChanged;

        public readonly string ID;
        public readonly string Title;
        public readonly string Description;
        
        public float TimeLimit { get; private set; } = 0f;
        
        
        public float rewardReputation { get; private set; } = 0.1f;
        public int rewardCash { get; private set; } = 0;
        public HashSet<ItemWithAmount> rewardItems { get; private set; } = new  HashSet<ItemWithAmount>();
        
        public QuestStatus Status { get; private set; } = QuestStatus.INACTIVE;
        
        private int _timerId = -1;
        
        /// <summary>
        /// Конструктор квеста, задаёт идентификатор и имя.
        /// </summary>
        /// <param name="id">Уникальный строковый идентификатор квеста.</param>
        /// <param name="title">Публичное строковое имя квеста</param>
        /// <param name="description">Публичное строковое описание квеста</param>
        public QuestOrderBase(string id, string title, string description)
        {
            this.ID = id;
            this.Title = title;
            this.Description = description;
        }
        
        /// <summary>
        /// Метод, стартующий квест. Если задан таймер - подписывается на игровое время.
        /// </summary>
        /// <param name="timeLimit">Параметр, означающий лимит времени для выполнения квеста. При истечении срока происходит автоматический провал. Использует мировое время.</param>
        /// <exception cref="WarningException">Нельзя переиспользовать старые квесты, не рекомендуется редактировать.</exception>
        /// <remarks>Вызывать строго в конце переопределяющего метода.</remarks>
        public virtual void StartQuest(float timeLimit = 0f)
        {
            if (Status == QuestStatus.ACTIVE)
            {
                throw new WarningException("Quest is already started");
            }
            if (Status != QuestStatus.INACTIVE)
            {
                throw new WarningException("Don't reuse old quest, you need create new");
            }
        
            TimeLimit = timeLimit;
            if(!Mathf.Approximately(TimeLimit, 0)) {
                _timerId = TimerService.Instance.AddTimer(TimeLimit, FailQuest, f => OnRemainingTimeChanged?.Invoke(f));
            }
            
            Status = QuestStatus.ACTIVE;
            OnStarted?.Invoke(this);
        }
        
        /// <summary>
        /// Провал квеста. Завершает его и останавливает таймеры.
        /// </summary>
        /// <exception cref="WarningException">Нельзя завершить не активный квест.</exception>
        /// <remarks>Вызывать строго в конце переопределяющего метода.</remarks>
        public virtual void FailQuest()
        {
            if (Status != QuestStatus.ACTIVE)
            {
                throw new WarningException("Quest is inactive");
            }

            Status = QuestStatus.FAILED;

            OnFailed?.Invoke(this);
            
            TimerService.Instance.RemoveTimer(_timerId);
            
            TavernReputationManager.Instance.DownReputation(rewardReputation/2);
        }
        
        /// <summary>
        /// Успешное завершение квеста. Завершает его и останавливает таймеры.
        /// </summary>
        /// <exception cref="WarningException">Нельзя завершить не активный квест.</exception>
        /// <remarks>Вызывать строго в конце переопределяющего метода.</remarks>
        public virtual void CompleteQuest()
        {
            if (Status != QuestStatus.ACTIVE)
            {
                throw new WarningException("Quest is inactive");
            }

            Status = QuestStatus.COMPLETED;
            
            TimerService.Instance.RemoveTimer(_timerId);
            
            TavernReputationManager.Instance.UpReputation(rewardReputation);
            
            OnCompleted?.Invoke(this);
            //InvokeCompleted();
        }

        /// <summary>
        /// Отмена квеста. Завершает его и останавливает таймеры.
        /// </summary>
        /// <exception cref="WarningException">Нельзя отменить не активный квест.</exception>
        /// <remarks>Вызывать строго в конце переопределяющего метода.</remarks>
        public virtual void CancelQuest()
        {
            if (Status != QuestStatus.ACTIVE)
            {
                throw new WarningException("Quest is inactive");
            }

            Status = QuestStatus.CANCELLED;
            
            OnCancelled?.Invoke(this);
            
            TimerService.Instance.RemoveTimer(_timerId);
            
            TavernReputationManager.Instance.DownReputation(rewardReputation/4);
        }

        /// <summary>
        /// Устанавливает новое ограничение по времени для квеста. Начинает таймер заново. Нулевое значение лимита убирает таймер вообще.
        /// </summary>
        /// <param name="timeLimit">Новый лимит времени.</param>
        /// <exception cref="ArgumentException">Лимит времени не может быть меньше нуля.</exception>
        public void SetNewTimeLimit(float timeLimit)
        {
            if (timeLimit < 0f)
            {
                throw new ArgumentException("Time limit must be greater than or equal to 0");
            }

            if (_timerId == -1)
            {
                TimerService.Instance.RemoveTimer(_timerId);
            }

            TimeLimit = timeLimit;
            
            _timerId = TimerService.Instance.AddTimer(TimeLimit, FailQuest);
        }
        
        public abstract bool ConditionCheck();
        
        public void SetReward(int cash, HashSet<ItemWithAmount> items = null, float rep = 0f)
        {
            rewardCash = cash;
            rewardItems.Clear();
            if (items != null)
            {
                rewardItems.AddRange(items);
            }

            rewardReputation = rep;
        }

        //protected abstract void InvokeCompleted();
    }
}