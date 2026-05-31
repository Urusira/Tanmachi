using System;
using System.ComponentModel;
using ShiroGe.Scripts.World;

namespace ShiroGe.Scripts.Quests
{
    public abstract class QuestOrderBase
    {
        public event System.Action<QuestOrderBase> OnStarted;
        public event System.Action<QuestOrderBase> OnFailed;
        public event System.Action<QuestOrderBase> OnCompleted;
        public event System.Action<QuestOrderBase> OnCancelled;

        public readonly string ID;
        public readonly string Title;
        public readonly string Description;

        public float TimeLimit { get; private set; } = 0f;
        public float Timer { get; private set; }
        
        public QuestStatus Status { get; private set; } = QuestStatus.INACTIVE;
        
        private bool _timeSubscribed = false;

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
            Timer = 0f;
            Status = QuestStatus.ACTIVE;
            OnStarted?.Invoke(this);
            
            TimeSubscribeUpdate();
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
            
            TimeSubscribeUpdate();
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
            
            OnCompleted?.Invoke(this);
            
            TimeSubscribeUpdate();
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
            
            TimeSubscribeUpdate();
        }

        /// <summary>
        /// Устанавливает новое ограничение по времени для квеста. Начинает таймер заново. Нулевое значение лимита убирает таймер вообще.
        /// </summary>
        /// <param name="timeLimit">Новый лимит времени.</param>
        /// <exception cref="ArgumentException">Лимит времени не может быть меньше нуля.</exception>
        public void SetTimeLimit(float timeLimit)
        {
            if (timeLimit < 0f)
            {
                throw new ArgumentException("Time limit must be greater than or equal to 0");
            }

            Timer = 0;
            TimeLimit = timeLimit;

            TimeSubscribeUpdate();
        }

        /// <summary>
        /// Обработчик для подписки на изменение мирового времени.
        /// </summary>
        /// <param name="deltaTime">Дельта мирового времени.</param>
        private void DeltaTimeTickHandler(float deltaTime)
        {
            if (Status != QuestStatus.ACTIVE || TimeLimit <= 0f) return;
            
            Timer += deltaTime;
            if (Timer > TimeLimit)
            {
                FailQuest();
            }
        }

        /// <summary>
        /// Обновляет статус подписки на мировое время. Отписывает, при нулевом лимите времени и наличии подписки и наоборот.
        /// </summary>
        /// <exception cref="ArgumentException">Временной лимит не может быть меньше нуля.</exception>
        private void TimeSubscribeUpdate()
        {
            if (TimeLimit < 0f)
                throw new ArgumentException("Time limit must be greater than or equal to 0");
            
            if (TimeLimit == 0f && _timeSubscribed)
            {
                TimeManager.Instance.OnDeltaTimeTick -= DeltaTimeTickHandler;
                _timeSubscribed = false;
                return;
            }
            
            if (TimeLimit > 0f && !_timeSubscribed)
            {
                TimeManager.Instance.OnDeltaTimeTick += DeltaTimeTickHandler;
                _timeSubscribed = true;
                return;
            }

            return;
        }


        public abstract bool ConditionCheck();
    }
}