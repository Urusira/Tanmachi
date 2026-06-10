using System;
using UnityEngine;

namespace ShiroGe.Scripts.Tavern
{
    [DefaultExecutionOrder(2)]
    public class TavernReputationManager : MonoBehaviour
    {
        public event System.Action<float> OnReputationChange;
        public event System.Action<float> OnReputationUpped;
        public event System.Action<float> OnReputationLowered;
        
        public static TavernReputationManager Instance { get; private set; }

        [field: SerializeField] public float MinReputation { get; private set; } = 0.0f;
        [field: SerializeField] public float MaxReputation { get; private set; } = 10.0f;
        
        private float _reputation = 0.0f;

        public float CurrentReputation
        {
            get => _reputation;
            private set => _reputation = Mathf.Clamp((Mathf.Round(value * 10f) / 10f), MinReputation, MaxReputation);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void UpReputation(float value)
        {
            CurrentReputation += value;
            
            OnReputationUpped?.Invoke(CurrentReputation);
            OnReputationChange?.Invoke(CurrentReputation);
        }

        public void DownReputation(float value)
        {
            CurrentReputation -= value;
            
            OnReputationLowered?.Invoke(CurrentReputation);
            OnReputationChange?.Invoke(CurrentReputation);
        }

        public void MaxReputationUpdate(float newMaximum)
        {
            MaxReputation = newMaximum;
            
            if(CurrentReputation > MaxReputation) CurrentReputation = MaxReputation;
        }

        public void MinReputationUpdate(float newMinimum)
        {
            MinReputation = newMinimum;
            
            if(CurrentReputation < MinReputation) CurrentReputation = MinReputation;
        }
    }
}