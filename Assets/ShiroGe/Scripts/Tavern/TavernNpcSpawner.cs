using System;
using System.Collections;
using System.Collections.Generic;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Utils;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ShiroGe.Scripts.Tavern
{
    [DefaultExecutionOrder(30)]
    [RequireComponent(typeof(NPCFabricator))]
    public class TavernNpcSpawner : MonoBehaviour
    {
        public float spawnIntervalSeconds = 5f;
        [Min(0f)] public float randomIntervalMin = 0f;
        [Min(0f)] public float randomIntervalMax = 3f;
        public bool randomizedSpawn = true;
        
        [SerializeField] private float spawnRadius = 2f;
        
        [Range(0f, 1f)][SerializeField] private float lonerSpawnChance = 0.5f;      // 50%
        [Range(0f, 1f)][SerializeField] private float baseEnterChance = 0.5f;      // 50%
        [Range(0f, 1f)][SerializeField] private float lonerChance = 0.3f;          // 30%
        [Range(0f, 1f)][SerializeField] private float groupChance = 0.4f;          // 40%

        private List<NPCController> _spawnedNPCs = new List<NPCController>();
        
        public int spawnMaxAmount = 50;
        
        public bool spawnOn { get; private set; }  = false;

        public GameObject spawnedEntityTargetGoing;
        
        private NPCFabricator _npcFabricator;

        private RepeatedTimer _spawnTimer = null;
        
        private bool hasPlaceForCustomer = false;
        private bool hasFullFreePlace = false;

        private bool _tavernOpened = true;
        
        private void Awake()
        {
            _npcFabricator = GetComponent<NPCFabricator>();
            SpawnerOn();
            TavernController.Instance.OnTavernOpen += OnTavernOpenHandler;
            TavernController.Instance.OnTavernClose += OnTavernCloseHandler;

            bool successful = SpawnersManager.Instance.SpawnerRegister(this);
            if(!successful) { print("Cannot register spawner in manager!" + gameObject + "\n" + gameObject.name); }
        }

        public void SpawnerOn()
        {
            
            if (spawnOn || _spawnTimer != null)
            {
                Debug.Log($"You trying to start already active spawner, cancelling operation...\nspawnOn: {spawnOn}, _spawnTimerId: {_spawnTimer}");
                return;
            }
            
            _spawnTimer = new RepeatedTimer(spawnIntervalSeconds, SpawnNpc, null);
            _spawnTimer.Start();
            
            spawnOn = true;
            Debug.Log($"SpawnerOn end\nspawnOn: {spawnOn}, _spawnTimerId: {_spawnTimer}");
        }

        public void SpawnerOff()
        {
            Debug.Log($"SpawnerOff start\nspawnOn: {spawnOn}, _spawnTimerId: {_spawnTimer}");
            if (!spawnOn || _spawnTimer == null)
            {
                Debug.Log($"You trying to stop already active spawner, cancelling operation...\nspawnOn: {spawnOn}, _spawnTimerId: {_spawnTimer}");
                return;
            }

            _spawnTimer.Stop();
            _spawnTimer = null;
            spawnOn = false;
            Debug.Log($"SpawnerOff end\nspawnOn: {spawnOn}, _spawnTimerId: {_spawnTimer}");
        }

        private void TavernTablesCheck()
        {
            int complFreeTables = TavernTablesManager.Instance.CompletlyFreeTables;
            int freePlaces = TavernTablesManager.Instance.FreePlaces;

            if (freePlaces > 0) hasPlaceForCustomer = true;
            else hasPlaceForCustomer = false;

            if (complFreeTables > 0)
            {
                hasFullFreePlace = true;
            }
            else
            {
                hasFullFreePlace = false;
            }
        }

        private void SpawnNpc()
        {
            if (!spawnOn)
            {
                Debug.LogWarning($"Looped timer not deleted yet, if this once warning after spawner disabling - ingore it.");
                return;
            }
            if (spawnOn && _spawnedNPCs.Count >= spawnMaxAmount)
            {
                SpawnerOff();
                return;
            }

            NPCController spawnedNpc;
            
            Vector3 randomSpawnOffset = Random.insideUnitSphere * spawnRadius;
            randomSpawnOffset.y = 0;
            Vector3 spawnPos = transform.position + randomSpawnOffset;
            
            if (spawnedEntityTargetGoing != null)
            {
                int seq = TavernNPCSequence.Get;
                spawnedNpc = _npcFabricator.NpcSpawn(
                    spawnPos,
                    new NPCData(
                        id: $"TavernNpc{seq}",
                        name: $"Посетитель{seq}",
                        personality: "Обычный доходяга.",
                        age: Random.Range(18, 100), 
                        introvert: Random.value <= lonerSpawnChance),
                        targetPos: spawnedEntityTargetGoing.transform.position
                    );
                
                spawnedNpc.SetBaseGoingTarget(spawnedEntityTargetGoing.transform.position);
                
            }
            else
            {
                int seq = TavernNPCSequence.Get;
                spawnedNpc = _npcFabricator.NpcSpawn(
                    spawnPos,
                    new NPCData(
                        id: $"TavernNpc{seq}", 
                        name: $"Посетитель{seq}",
                        personality: "Обычный доходяга.", 
                        age: Random.Range(18, 100), 
                        introvert: Random.value <= lonerSpawnChance)
                    );
            }

            spawnedNpc.SetRandomAvoidancyPriority();
            _spawnedNPCs.Add(spawnedNpc);
            spawnedNpc.OnNpcDestroyed += OnNpcDestroyedHandler;
            SetNpcStrategy(spawnedNpc);
        }

        private void SpawnGroupNpc(NPCController leader, int amount, TavernTable table = null)
        {
            List<NPCController> spawnedNpcs = new List<NPCController> { leader };
            
            Vector3 randomSpawnOffset = Random.insideUnitSphere * spawnRadius;
            randomSpawnOffset.y = 0;
            Vector3 spawnPos = leader.transform.position + randomSpawnOffset;
            
            for (int i = 0; i < amount; i++)
            {
                int seq = TavernNPCSequence.Get;
                NPCController spawnedNpc;
                spawnedNpc = _npcFabricator.NpcSpawn(
                    spawnPos,
                    new NPCData(
                        id: $"TavernNpc{seq}", 
                        name: $"Посетитель{seq}",
                        personality: "Обычный доходяга.", 
                        age: Random.Range(18, 100), 
                        introvert: Random.value <= lonerSpawnChance)
                );

                if (table != null)
                {
                    spawnedNpc.GoToTavernWithGroup(table);
                }
                
                spawnedNpc.SetBaseGoingTarget(spawnedEntityTargetGoing.transform.position);
                
                spawnedNpc.SetRandomAvoidancyPriority();
                
                spawnedNpcs.Add(spawnedNpc);
                
                spawnedNpc.OnNpcDestroyed += OnNpcDestroyedHandler;
            }
            
            foreach (NPCController npc in spawnedNpcs)
            {
                npc.SetGroup(spawnedNpcs);
            }
        }

        private void SetNpcStrategy(NPCController spawnedNpc)
        {
            if (!_tavernOpened)
            {
                return;
            }
            
            TavernTablesCheck();
            
            if(Random.value <= baseEnterChance)
            {
                //Сценарий, когда есть свободные места
                if (hasPlaceForCustomer)
                {
                    //Если есть полностью свободное место
                    if (hasFullFreePlace)
                    {
                        //Если нпс интроверт
                        if (spawnedNpc.NpcData.Introvert)
                        {
                            if (Random.value <= lonerChance)
                            {
                                //Интроверт садится за полностью пустой стол
                                CyclicNpcTableSendler(spawnedNpc, 1, true, false, false);
                            }
                        }
                        else //Если нпс не интроверт
                        {
                            if (Random.value <= groupChance)
                            {
                                //Нпс спавнит группу и они идут за пустой стол
                                CyclicNpcTableSendler(spawnedNpc, 1, true, true, true);
                            }
                            else
                            {
                                //Группа не прокнула. Тогда чел просто пойдёт один и к кому-нибудь подсядет
                                CyclicNpcTableSendler(spawnedNpc, 1, false, false, false);
                            }
                        }
                    }
                    else //Полностью свободных мест нет, но в принципе свободные места есть
                    {
                        // Если чел не одиночка
                        if (spawnedNpc.NpcData.Introvert == false)
                        {
                            CyclicNpcTableSendler(spawnedNpc, 1, false, false, false);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private bool CyclicNpcTableSendler(NPCController spawnedNpc, int reqPlacesAmount, bool needCompletlyFreePlace, bool groupped, bool groupLeader)
        {
            TavernTable table = TavernTablesManager.Instance.GetAvailableTable(reqPlacesAmount, needCompletlyFreePlace);
            
            if (table != null)
            {
                bool successful = spawnedNpc.GoToTavern(table, groupLeader);
                                    
                while(!successful)
                {
                    table = TavernTablesManager.Instance.GetAvailableTable(reqPlacesAmount, needCompletlyFreePlace);
                    if(table != null)
                    {
                        successful = spawnedNpc.GoToTavern(table, groupLeader);
                    }
                    else
                    {
                        if(spawnedEntityTargetGoing)
                            spawnedNpc.SetGoingTarget(spawnedEntityTargetGoing.transform.position);
                        else
                        {
                            spawnedNpc.GoWandering();
                        }
                        break;
                    }
                }
                
                if (successful)
                {
                    if(groupped)
                    {
                        SpawnGroupNpc(spawnedNpc, table.AmountAvailablePlaces, table);
                    }

                    return true;
                }
            }

            return false;
        }
        
        private void OnNpcDestroyedHandler(NPCController npc)
        {
            _spawnedNPCs.Remove(npc);
            npc.OnNpcDestroyed -= OnNpcDestroyedHandler;
    
            if (!spawnOn && _spawnedNPCs.Count < spawnMaxAmount)
            {
                SpawnerOn();
            }
        }

        private void OnTavernOpenHandler()
        {
            _tavernOpened = true;
        }
        
        private void OnTavernCloseHandler()
        {
            foreach (NPCController npc in _spawnedNPCs)
            {
                npc.CancelGoingToTavern();
            }

            _tavernOpened = false;
        }
    }
}