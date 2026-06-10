using System;
using System.Collections.Generic;
using ShiroGe.Scripts.Tavern;
using UnityEngine;

namespace ShiroGe.Scripts.World
{
    public class SpawnersManager : MonoBehaviour
    {
        public static SpawnersManager Instance { get; private set; }
        public HashSet<TavernNpcSpawner> Spawners { get; private set; } = new HashSet<TavernNpcSpawner>();
        
        [SerializeField] private bool _disableSpawnersAtNight = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        public bool SpawnerRegister(TavernNpcSpawner spawner)
        {
            if (Spawners.Contains(spawner)) return false;
            
            Spawners.Add(spawner);
            
            if (Spawners.Contains(spawner)) return true;
            else return false;
        }

        private void Start()
        {
            TimeManager.Instance.OnDayPhaseChanged += OnDayPhaseChangedHandler;
        }

        private void OnDayPhaseChangedHandler(DayPhase dayPhase)
        {
            if (!_disableSpawnersAtNight) return;
            switch (dayPhase)
            {
                case DayPhase.Day:
                {
                    foreach (TavernNpcSpawner spawner in Spawners)
                    {
                        spawner.SpawnerOn();
                    }
                    break;
                }
                case DayPhase.Night:
                {
                    foreach (TavernNpcSpawner spawner in Spawners)
                    {
                        spawner.SpawnerOff();
                    }
                    break;
                }
            }
        }
    }
}