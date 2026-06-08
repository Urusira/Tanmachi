using System.Collections.Generic;
using System.Linq;
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
        
        public int AddTimer(float duration, System.Action onComplete, bool loop = false, bool randomized = false, float randomMin = 0f, float randomMax = 0f)
        {
            float finalDuration = duration;
            if (randomized)
            {
                if (randomMax <= randomMin)
                {
                    Debug.Log("Randomized timer: random max cannot be over or equal random min. Min has dropped to 0");
                    randomMin = 0f;
                }

                if (randomMin == 0f && randomMax == 0f)
                {
                    Debug.Log("Randomized timer: randomize values cannot be zero, cancelling randomization");
                    randomized = false;
                }

                if (randomMax > duration)
                {
                    Debug.Log("Randomized timer: random max cannot be over duration, dropping random max to duration");
                    randomMax = duration;
                }
            }
            
            ActiveTimer newTimer = new ActiveTimer
            {
                TimerId = _nextTimerId++,
                Remaining = finalDuration,
                InitialDuration = finalDuration,
                OnComplete = onComplete,
                Loop = loop,
                Randomized = randomized,
                RandomMin = randomMin,
                RandomMax = randomMax
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
    
        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                ActiveTimer timer = _timers[i];
                timer.Remaining -= delta;
    
                if (timer.Remaining <= 0)
                {
                    timer.OnComplete?.Invoke();
        
                    if (timer.Loop)
                    {
                        if (timer.Randomized)
                        {
                            timer.Remaining = timer.InitialDuration + Random.Range(timer.RandomMin, timer.RandomMax);
                        }
                        else
                        {
                            timer.Remaining = timer.InitialDuration;
                        }
                        _timers[i] = timer;
                    }
                    else
                    {
                        _timers.RemoveAt(i);
                    }
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
            public System.Action OnComplete;
            public bool Loop;
            public bool Randomized;
            public float RandomMin;
            public float RandomMax;
        }
    }
}