using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public abstract class Wearable : MonoBehaviour
    {
        //public GameObject onPlayerPrefab;
        public bool equipped = false;

        public virtual void Equip(GameObject TargetEquipment)
        {
            //if(onPlayerPrefab != null) onPlayerPrefab.SetActive(true);
            equipped = true;
        }
        
        public virtual void Unequip(GameObject TargetEquipment)
        {
            //if(onPlayerPrefab != null) onPlayerPrefab.SetActive(false);
            equipped = false;
        }
    }
}