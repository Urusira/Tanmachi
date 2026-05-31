using System;
using UnityEngine;

namespace ShiroGe.Scripts.World
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        
        public event System.Action<float> OnTimeTick;
        public event System.Action<float> OnDeltaTimeTick;
        
        public float currentTime { get; private set; }
        public int currentDay  { get; private set; } = 0;
        public float timeFactor { get; private set; } = 1f;
        
        [SerializeField] private float dayLength = 36000f;
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
            currentTime = initialTime;
            _dayNightCycle = GetComponent<DayNightCycleManager>();
        }
        
        private void FixedUpdate()
        {
            float lateTime = currentTime;
            currentTime += 1*timeFactor;

            if (currentTime >= dayLength)
            {
                currentTime++;
            }
            
            currentTime = currentTime % dayLength;
            
            _dayNightCycle.SetSunTimeRotation(currentTime);
            
            OnTimeTick?.Invoke(currentTime);
            OnDeltaTimeTick?.Invoke(currentTime-lateTime);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}