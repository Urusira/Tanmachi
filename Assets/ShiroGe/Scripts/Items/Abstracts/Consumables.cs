using ShiroGe.Scripts.Enums;
using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public abstract class Consumables : MonoBehaviour
    {
        public readonly ItemTypeEnum Type = ItemTypeEnum.CONSUMABLE;
        protected abstract void UseIt();
    }
}