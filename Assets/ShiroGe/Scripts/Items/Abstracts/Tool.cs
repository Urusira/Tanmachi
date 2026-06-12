using ShiroGe.Scripts.Enums;
using UnityEngine;

namespace ShiroGe.Scripts.Items
{
    public abstract class Tool : MonoBehaviour
    {
        public readonly ItemTypeEnum Type = ItemTypeEnum.TOOL;
    }
}