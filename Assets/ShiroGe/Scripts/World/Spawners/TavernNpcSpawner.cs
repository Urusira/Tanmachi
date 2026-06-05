using System;
using System.Collections;
using ShiroGe.Scripts.NPC;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ShiroGe.Scripts.World.Spawners
{
    [RequireComponent(typeof(NPCFabricator))]
    public class TavernNpcSpawner : MonoBehaviour
    {
        public float spawnRateSeconds = 3f;
        public bool spawnOn = true;
        public int spawnMaxAmount = 20;
        
        private NPCFabricator _npcFabricator;

        private int _npcNumberSequence = -1;
        
        private void Awake()
        {
            _npcFabricator = GetComponent<NPCFabricator>();
            SpawnerOn();
        }

        public void SpawnerOn()
        {
            spawnOn = true;
            StartCoroutine(SpawnerCoroutine());
        }

        public void SpawnerOff()
        {
            spawnOn = false;
            StopCoroutine(SpawnerCoroutine());
        }

        private IEnumerator SpawnerCoroutine()
        {
            while (spawnOn)
            {
                if (_npcNumberSequence >= spawnMaxAmount)
                {
                    spawnOn = false;
                    break;
                }
                NPCController spawnedNpc = _npcFabricator.NpcSpawn(transform.position, new NPCData($"TavernNpc{++_npcNumberSequence}", $"Посетитель{++_npcNumberSequence}", "Обычный доходяга.", Random.Range(18, 100), Random.Range(0, 1) == 1));
                spawnedNpc.GoingToTavern = true;
                yield return new WaitForSecondsRealtime(spawnRateSeconds);
            }
        }
    }
}