using System.Collections.Generic;
using UnityEngine;

namespace ShiroGe.Scripts.NPC
{
    public class NPCFabricator : MonoBehaviour
    {
        [SerializeField] private GameObject npcPrefab;

        public NPCController NpcSpawn(Vector3 spawnPos, NPCData npcData)
        {
            GameObject spawnedEntity = Instantiate(npcPrefab, spawnPos, new Quaternion(0f, 0f, 0f, 0f));

            NPCController spawnedController;
            spawnedEntity.TryGetComponent<NPCController>(out spawnedController);
            if (spawnedController == null) 
            {
                spawnedController = spawnedEntity.AddComponent<NPCController>();
            }
            
            spawnedController.SetNpcData(npcData);
            
            return spawnedController;
        }
        
        public NPCController NpcSpawnWithTarget(Vector3 spawnPos, Vector3 targetPos, NPCData npcData)
        {
            GameObject spawnedEntity = Instantiate(npcPrefab, spawnPos, new Quaternion(0f, 0f, 0f, 0f));

            NPCController spawnedController;
            spawnedEntity.TryGetComponent<NPCController>(out spawnedController);
            if (spawnedController == null)
            {
                spawnedController = spawnedEntity.AddComponent<NPCController>();
            }
            
            spawnedController.SetNpcData(npcData);
            spawnedController.SetGoingTarget(targetPos);
            
            return spawnedController;
        }
    }
}