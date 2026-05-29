using ShiroGe.CharacterController;
using ShiroGe.Scripts.Items;
using UnityEngine;

public class NoviceArmorLegs : LegsWear
{
    public override void Equip(GameObject TargetEquipment)
    {
        base.Equip(TargetEquipment);
        EntityArmorsController PAC = TargetEquipment.GetComponent<EntityArmorsController>();
        if(PAC != null) PAC.noviceArmorLegs.SetActive(true);
    }
        
    public override void Unequip(GameObject TargetEquipment)
    {
        base.Unequip(TargetEquipment);
        EntityArmorsController PAC = TargetEquipment.GetComponent<EntityArmorsController>();
        if(PAC != null) PAC.noviceArmorLegs.SetActive(false);
    }
}
