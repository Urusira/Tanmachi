using System;
using System.Collections.Generic;
using ShiroGe.Scripts.UI;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance  { get; private set; }
    
    public GameObject hotbarObj;
    public GameObject inventorySlotParent;
    
    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();
    private List<InventorySlot> _hotbarSlots = new List<InventorySlot>();
    private List<InventorySlot> _allSlots = new List<InventorySlot>();

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    private void Awake()
    {
        _inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<InventorySlot>());
        _hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<InventorySlot>());
        
        _allSlots.AddRange(_inventorySlots);
        _allSlots.AddRange(_hotbarSlots);
    }

    private void Update()
    {
        
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        foreach (InventorySlot slot in _allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmout = slot.GetItemAmount();
                int maxStack = itemToAdd.maxStackSize;
                if (currentAmout < maxStack)
                {
                    int spaceLeft = maxStack - currentAmout;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);
                    
                    slot.SetItem(itemToAdd, currentAmout+amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0)
                        return;
                }
            }
        }

        foreach (InventorySlot slot in _allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;
                
                if(remaining <= 0)
                    return;
            }
        }

        if (remaining > 0)
        {
            Debug.Log($"Inventory Full, could not add {remaining} of {itemToAdd.itemName}");
        }
    }
}
