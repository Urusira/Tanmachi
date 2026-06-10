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
            
            if (spawnedEntityTargetGoing != null)
            {
                int seq = TavernNPCSequence.Get;
                spawnedNpc = _npcFabricator.NpcSpawnWithTarget(
                    transform.position,
                    spawnedEntityTargetGoing.transform.position,
                    new NPCData(
                        $"TavernNpc{seq}", 
                        $"Посетитель{seq}",
                        "Обычный доходяга.",
                        Random.Range(18, 100), 
                        Random.value <= lonerSpawnChance)
                    );
                
                spawnedNpc.SetBaseGoingTarget(spawnedEntityTargetGoing.transform.position);
            }
            else
            {
                int seq = TavernNPCSequence.Get;
                spawnedNpc = _npcFabricator.NpcSpawn(
                    transform.position,
                    new NPCData(
                        $"TavernNpc{seq}", 
                        $"Посетитель{seq}",
                        "Обычный доходяга.", 
                        Random.Range(18, 100), 
                        Random.value <= lonerSpawnChance)
                    );
            }

            _spawnedNPCs.Add(spawnedNpc);
            spawnedNpc.OnNpcDestroyed += OnNpcDestroyedHandler;
            SetNpcStrategy(spawnedNpc);
        }

        private void SpawnGroupNpc(int amount, TavernTable table = null)
        {
            for (int i = 0; i < amount; i++)
            {
                int seq = TavernNPCSequence.Get;
                NPCController spawnedNpc;
                spawnedNpc = _npcFabricator.NpcSpawn(
                    transform.position,
                    new NPCData(
                        $"TavernNpc{seq}", 
                        $"Посетитель{seq}",
                        "Обычный доходяга.", 
                        Random.Range(18, 100), 
                        Random.value <= lonerSpawnChance)
                );

                if (table != null)
                {
                    spawnedNpc.GoToTavernWithGroup(table);
                }
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
                        if (spawnedNpc.NpcData.Loner)
                        {
                            if (Random.value <= lonerChance)
                            {
                                //Интроверт садится за полностью пустой стол
                                CyclicNpcTableSendler(spawnedNpc, 1, true, false);
                            }
                        }
                        else //Если нпс не интроверт
                        {
                            if (Random.value <= groupChance)
                            {
                                //Нпс спавнит группу и они идут за пустой стол
                                CyclicNpcTableSendler(spawnedNpc, 1, true, true);
                            }
                            else
                            {
                                //Группа не прокнула. Тогда чел просто пойдёт один и к кому-нибудь подсядет
                                CyclicNpcTableSendler(spawnedNpc, 1, false, false);
                            }
                        }
                    }
                    else //Полностью свободных мест нет, но в принципе свободные места есть
                    {
                        // Если чел не одиночка
                        if (spawnedNpc.NpcData.Loner == false)
                        {
                            CyclicNpcTableSendler(spawnedNpc, 1, false, false);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private bool CyclicNpcTableSendler(NPCController spawnedNpc, int reqPlacesAmount, bool needCompletlyFreePlace, bool groupped)
        {
            TavernTable table = TavernTablesManager.Instance.GetAvailableTable(reqPlacesAmount, needCompletlyFreePlace);
            
            if (table != null)
            {
                bool successful = spawnedNpc.GoToTavern(table);
                                    
                while(!successful)
                {
                    table = TavernTablesManager.Instance.GetAvailableTable(reqPlacesAmount, needCompletlyFreePlace);
                    if(table != null)
                    {
                        successful = spawnedNpc.GoToTavern(table);
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
                        SpawnGroupNpc(table.AmountAvailablePlaces, table);
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