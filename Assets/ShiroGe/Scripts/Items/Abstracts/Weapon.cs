using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public abstract class Weapon : MonoBehaviour
    {
        public readonly ItemTypeEnum Type = ItemTypeEnum.WEAPON;
        
        protected abstract void Attack();
    }
}