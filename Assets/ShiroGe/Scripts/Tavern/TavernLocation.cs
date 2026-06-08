using ShiroGe.Scripts.NPC;
using Unity.VisualScripting;
using UnityEngine;

namespace ShiroGe.Scripts.Tavern
{
    public class TavernLocation : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            NPCController npc = other.GetComponent<NPCController>();
            npc?.TavernEntry();
        }

        private void OnTriggerExit(Collider other)
        {
            NPCController npc = other.GetComponent<NPCController>();
            npc?.TavernLeave();
        }
    }
}