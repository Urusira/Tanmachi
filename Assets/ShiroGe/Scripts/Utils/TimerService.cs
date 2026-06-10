using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShiroGe.Scripts.World
{
    public class TimerService : MonoBehaviour
    {
        public static TimerService Instance { get; private set; }
        
        private List<ActiveTimer> _timers = new List<ActiveTimer>();
        private static int _nextTimerId = 1;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        public int AddTimer(float duration, Action onComplete, Action<float> onRemainingChanged = null)
        {
            ActiveTimer newTimer = new ActiveTimer
            {
                TimerId = _nextTimerId++,
                Remaining = duration,
                InitialDuration = duration,
                OnComplete = onComplete,
                OnRemainingChanged = onRemainingChanged,
            };
            
            _timers.Add(newTimer);
            return newTimer.TimerId;
        }
        
        public int AddTimerWithContext<T>(float duration, Action<T> onComplete, T context, 
            Action<float> onRemainingChanged = null)
        {
            ActiveTimer newTimer = new ActiveTimer
            {
                TimerId = _nextTimerId++,
                Remaining = duration,
                InitialDuration = duration,
                OnCompleteWithContext = (ctx) => onComplete((T)ctx),
                Context = context,
                OnRemainingChanged = onRemainingChanged,
            };
            
            _timers.Add(newTimer);
            return newTimer.TimerId;
        }
        
        public void RemoveTimer(int timerId)
        {
            for (int i = 0; i < _timers.Count; i++)
            {
                if (_timers[i].TimerId == timerId)
                {
                    _timers.RemoveAt(i);
                    return;
                }
            }
        }
        
        public bool TryGetRemainingTime(int timerId, out float remaining)
        {
            foreach (var timer in _timers)
            {
                if (timer.TimerId == timerId)
                {
                    remaining = timer.Remaining;
                    return true;
                }
            }
            remaining = 0f;
            return false;
        }
        
        private void Update()
        {
            float delta = Time.deltaTime;
            
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                ActiveTimer timer = _timers[i];
                
                float step = delta;
                timer.Remaining -= step;
                timer.OnRemainingChanged?.Invoke(timer.Remaining);
                
                if (timer.Remaining <= 0)
                {
                    if (timer.OnComplete != null)
                        timer.OnComplete.Invoke();
                    else if (timer.OnCompleteWithContext != null)
                        timer.OnCompleteWithContext.Invoke(timer.Context);
                    
                    _timers.RemoveAt(i);
                }
                else
                {
                    _timers[i] = timer;
                }
            }
        }
        
        private struct ActiveTimer
        {
            public int TimerId;
            public float Remaining;
            public float InitialDuration;
            public Action OnComplete;
            public Action<object> OnCompleteWithContext;
            public object Context;
            public Action<float> OnRemainingChanged;
        }
    }
}