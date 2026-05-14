using System;
using UnityEngine;

namespace ShiroGe.Scripts.World
{
    public class TimeManager : MonoBehaviour
    {
        public float timeFactor = 0.1f;
        public float currentTime;
        public float initialTime = 0f;
        public float dayLength = 3600f;
        public int currentDay = 0;
        
        private DayNightCycleManager _dayNightCycle;

        private void Start()
        {
            currentTime = initialTime;
            _dayNightCycle = GetComponent<DayNightCycleManager>();
        }
        
        private void FixedUpdate()
        {
            currentTime += 1*timeFactor;

            if (currentTime >= dayLength)
            {
                currentTime++;
            }
            
            currentTime = currentTime % dayLength;
            
            _dayNightCycle.SetSunTimeRotation(currentTime);
        }
        
    }
}