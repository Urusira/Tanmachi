using ShiroGe.Scripts.NPC;
using UnityEngine;

public class NPCDespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NPCController npc = other.GetComponent<NPCController>();
        if (npc != null) Destroy(npc.gameObject);
    }
}
