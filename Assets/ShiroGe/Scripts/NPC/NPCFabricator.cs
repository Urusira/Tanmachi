using System.Collections.Generic;
using ShiroGe.Scripts.Utils;
using UnityEngine;

namespace ShiroGe.Scripts.NPC
{
    public class NPCFabricator : MonoBehaviour
    {
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private NPCNamesDatabase nameDatabase;
        
        public NPCController NpcSpawn(Vector3 spawnPos, NPCData npcData, Vector3? targetPos = null)
        {
            SetNPCRandomName(npcData);
            
            GameObject spawnedEntity = Instantiate(npcPrefab, spawnPos, Quaternion.identity);

            NPCController spawnedController;
            spawnedEntity.TryGetComponent(out spawnedController);
            if (spawnedController == null)
            {
                spawnedController = spawnedEntity.AddComponent<NPCController>();
            }
            
            spawnedController.SetNpcData(npcData);
            if(targetPos.HasValue)
                spawnedController.SetGoingTarget(targetPos.Value);
            
            return spawnedController;
        }
        
        private void SetNPCRandomName(NPCData npcData)
        {
            string firstName = nameDatabase.firstNames[Random.Range(0, nameDatabase.firstNames.Length)];
            string lastName = nameDatabase.lastNames[Random.Range(0, nameDatabase.lastNames.Length)];
            
            npcData.SetNewName($"{firstName} {lastName}");
        }
    }
}