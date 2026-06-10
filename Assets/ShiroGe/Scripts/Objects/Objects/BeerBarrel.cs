using ShiroGe.CharacterController;
using ShiroGe.Scripts.Enums;
using ShiroGe.Scripts.Items;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Objects;
using UnityEngine;

public class BeerBarrel : Station
{
    [SerializeField] private ItemSO _emptyMugItem;
    [SerializeField] private ItemSO _fullyMugItem;
    [SerializeField] public CraftStations typeStation = CraftStations.BeerBarrel;
    
    protected override PlayerActionsState PlayerOverridableInteract(GameObject _)
    {
        if (InventoryManager.Instance.InventoryItemWithAmountCheck(_emptyMugItem, 1))
        {
            int removed = InventoryManager.Instance.RemoveItem(_emptyMugItem, 1);
            if(removed > 0) InventoryManager.Instance.AddItem(_fullyMugItem, 1);
            Debug.Log("Removed " + removed + " " + _emptyMugItem.itemName);
        }
        
        return PlayerActionsState.Default;
    }

    
    protected override NPCActionsState NpcOverridableInteract(GameObject npc)
    {
        throw new System.NotImplementedException();
    }

    protected override void Initiate()
    {
        return;
    }
    
    public override string ShowHint()
    {
        base.ShowHint();
        if (InventoryManager.Instance.InventoryItemCheck(_emptyMugItem))
        {
            return $"{name}\nF чтобы налить";
        }
        return $"{name}\nДля использования вам нужна пустая кружка";
    }
}
