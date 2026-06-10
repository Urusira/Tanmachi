using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShiroGe.Scripts.World
{
    public enum DayPhase
    {
        Day = 0,
        Night = 1
    }
    
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        
        public event System.Action<float> OnTimeTick;
        public event System.Action<float> OnDeltaTimeTick; 
        public event System.Action<DayPhase> OnDayPhaseChanged;
        public event System.Action OnDayChanged;
        
        [field: SerializeField] public float CurrentTime { get; private set; }
        [field: SerializeField] public float CurrentTimeFactor { get; private set; }
        [field: SerializeField] public float NormalTimeFactor { get; private set; } = 0.1f;
        [field: SerializeField] public float SleepingTimeFactor { get; private set; } = 2f;
        
        [field: SerializeField] public int hourCorrectiveOffset = 11;
        [field: SerializeField] public int minCorrectiveOffset = 0;
        [field: SerializeField] public int secCorrectiveOffset = 0;
        
        [field: SerializeField] public int CurrentDay  { get; private set; } = 0;
        
        private bool dayChanged = false;
        
        public float DeltaTime { get; private set; }
        
        public DayPhase CurrentDayPhase { get; private set; } = DayPhase.Day;

        public string NamedCurrentDayPhase
        {
            get
            {
                switch (CurrentDayPhase)
                {
                    case DayPhase.Day:
                    {
                        return "День";
                        break;
                    }
                    case DayPhase.Night:
                    {
                        return "Ночь";
                        break;
                    }
                }
                
                return "Проклятый День";
            }
        }

        public float DayPhaseSkipTime { get; private set; } = 10f;
        
        [SerializeField] private float dayLength = 36000f;
        [SerializeField] private float dayTiming = 0f;
        [SerializeField] private float nightTiming = 19000f;
        [SerializeField] private float midnightTiming = 25500f;
        [SerializeField] private float initialTime = 0f;
        
        private DayNightCycleManager _dayNightCycle;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            CurrentTime = initialTime;
            CurrentTimeFactor = NormalTimeFactor;
            _dayNightCycle = GetComponent<DayNightCycleManager>();
        }
        
        private void FixedUpdate()
        {
            float lateTime = CurrentTime;
            CurrentTime += 1*CurrentTimeFactor;

            if (CurrentTime >= midnightTiming && CurrentTime < dayLength && !dayChanged)
            {
                dayChanged = true;
                CurrentDay++;
                OnDayChanged?.Invoke();
            }

            if (CurrentTime >= dayLength) dayChanged = false;


            DayPhase newPhase = (CurrentTime >= nightTiming) ? DayPhase.Night : DayPhase.Day;
            if (newPhase != CurrentDayPhase)
            {
                CurrentDayPhase = newPhase;
                OnDayPhaseChanged?.Invoke(CurrentDayPhase);
            }
            
            CurrentTime = CurrentTime % dayLength;
            
            _dayNightCycle.SetSunTimeRotation((CurrentTime / dayLength) * 360);
            
            DeltaTime = CurrentTime - lateTime;
            
            OnTimeTick?.Invoke(CurrentTime);
            OnDeltaTimeTick?.Invoke(DeltaTime);
        }

        public int[] Get24FormattedTime()
        {
            int minsInCycle = 24 * 60;
            float ticksInMinute = dayLength / minsInCycle;

            float totalMinutes = CurrentTime / ticksInMinute;
    
            int currentHours = (Mathf.FloorToInt(totalMinutes / 60)+hourCorrectiveOffset)%24;
            int currentMins = (Mathf.FloorToInt(totalMinutes % 60)+minCorrectiveOffset)%60;
            int currentSeconds = (Mathf.FloorToInt((totalMinutes % 1) * 60)+secCorrectiveOffset)%60;
            
            return new []{CurrentDay+1, currentHours, currentMins, currentSeconds};
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SkipDayPhase()
        {
            StartCoroutine(SkipDayPhaseCoroutine());
        }

        private IEnumerator SkipDayPhaseCoroutine()
        {
            DayPhase oldPhase = CurrentDayPhase;
            CurrentTimeFactor = SleepingTimeFactor;
            
            while (CurrentDayPhase == oldPhase)
            {
                yield return null;
            }

            CurrentTimeFactor = NormalTimeFactor;
        }

        public void DayPhaseChange()
        {
            if (CurrentDayPhase == DayPhase.Day)
            {
                CurrentDayPhase = DayPhase.Night;
                CurrentTime = nightTiming;
            }
            else if (CurrentDayPhase == DayPhase.Night)
            {
                CurrentDayPhase = DayPhase.Day;
                CurrentTime = dayTiming;
            }
            
            OnDayPhaseChanged?.Invoke(CurrentDayPhase);
        }
    }
}