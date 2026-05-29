using ShiroGe.CharacterController;
using ShiroGe.Scripts.Items;
using UnityEngine;

public class NoviceArmorBody : BodyWear
{
    public override void Equip(GameObject TargetEquipment)
    {
        base.Equip(TargetEquipment);
        EntityArmorsController PAC = TargetEquipment.GetComponent<EntityArmorsController>();
        if(PAC != null) PAC.noviceArmorBody.SetActive(true);
    }
        
    public override void Unequip(GameObject TargetEquipment)
    {
        base.Unequip(TargetEquipment);
        EntityArmorsController PAC = TargetEquipment.GetComponent<EntityArmorsController>();
        if(PAC != null) PAC.noviceArmorBody.SetActive(false);
    }
}
