using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using ShiroGe.CharacterController;
using ShiroGe.Scripts;
using ShiroGe.Scripts.Enums;
using ShiroGe.Scripts.Items;
using ShiroGe.Scripts.Quests;
using ShiroGe.Scripts.UI;
using ShiroGe.Scripts.Utils;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.UI;


[DefaultExecutionOrder(2)]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance  { get; private set; }

    public event System.Action<InventoryManager> OnInventoryChanged;
    
    [SerializeField] private GameObject hotbarObj;
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject armorSlotsObj;

    [SerializeField] private InventorySlot dragSlot;

    [SerializeField] private Vector2 dragSlotOffset = new Vector2(90, -90);
    
    public List<InventorySlot> _inventorySlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _hotbarSlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _armorSlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _allSlots { get; private set; } = new List<InventorySlot>();
    
    public InventorySlot HoveredSlot { get; private set; }
    
    [SerializeField] private List<ItemWithAmount> _startingInventory;

    public int SelectedHotbarSlot { get; private set; } = 0;

    public bool IsDragging { get; private set; } = false;
    
    private Dictionary<ItemSO, int> _inventoryAllItems = new Dictionary<ItemSO, int>();
    private InventorySlot _draggedSlot = null;
    private bool _quickTransfer = false;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        //DontDestroyOnLoad(gameObject);

        Instance = this;

        if (_startingInventory.Count > 0)
        {
            foreach (ItemWithAmount item in _startingInventory)
            {
                AddItem(item.Item, item.Amount);
            }
        }
        
        RebuildInventoryCache();
    }

    private void Awake()
    {
        _inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<InventorySlot>());
        _hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<InventorySlot>());
        _armorSlots.AddRange(armorSlotsObj.GetComponentsInChildren<InventorySlot>());
        
        _allSlots.AddRange(_hotbarSlots);
        _allSlots.AddRange(_inventorySlots);
        _allSlots.AddRange(_armorSlots);
    }

    private void Update()
    {
        if(IsDragging)
            UpdateDragItemPosition();
    }

    public int AddItem(ItemSO itemToAdd, int amount)
    {
        return AddItem(itemToAdd, amount, _allSlots);
    }
    
    private int AddItem(ItemSO itemToAdd, int amount, List<InventorySlot> targetInventoryZone)
    {
        bool quickTransfer = _quickTransfer;
        int remaining = amount;

        foreach (InventorySlot slot in targetInventoryZone)
        {
            if (!slot.HasItem() || slot.GetItem() != itemToAdd || slot.specialType != ItemTypeEnum.DEFAULT) continue;
        
            int spaceLeft = itemToAdd.maxStackSize - slot.GetItemAmount();
            if (spaceLeft <= 0) continue;
        
            int toAdd = Mathf.Min(spaceLeft, remaining);
            slot.AddAmount(toAdd);
            remaining -= toAdd;
        
            if (remaining <= 0) break;
        }

        if (remaining > 0)
        {
            foreach (InventorySlot slot in targetInventoryZone)
            {
                if (slot.HasItem() || slot.specialType != ItemTypeEnum.DEFAULT) continue;
            
                int toAdd = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, toAdd);
                remaining -= toAdd;
            
                if (remaining <= 0) break;
            }
        }

        int actuallyAdded = amount - remaining;
        if (actuallyAdded > 0 && !quickTransfer)
        {
            if (_inventoryAllItems.ContainsKey(itemToAdd)) _inventoryAllItems[itemToAdd] += actuallyAdded;
            else _inventoryAllItems.Add(itemToAdd, actuallyAdded);
        }
    
        ItemRedraw();
            
        if (remaining > 0)
            Debug.Log($"Inventory Full, could not add {remaining} of {itemToAdd.itemName}");
    
        return remaining;
    }
    
    public int RemoveItem(ItemSO item, int amount)
    {
        int remainingToRemove = amount;
        int totalRemoved = 0;
    
        List<InventorySlot> slotsWithItem = new List<InventorySlot>();
        foreach (InventorySlot slot in _allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == item)
            {
                slotsWithItem.Add(slot);
            }
        }
    
        foreach (InventorySlot slot in slotsWithItem)
        {
            int slotAmount = slot.GetItemAmount();
        
            if (slotAmount <= remainingToRemove)
            {
                totalRemoved += slotAmount;
                remainingToRemove -= slotAmount;
                slot.ClearSlot();
            }
            else
            {
                slot.RemoveAmount(remainingToRemove);
                totalRemoved += remainingToRemove;
                remainingToRemove = 0;
            }
        
            if (remainingToRemove <= 0)
                break;
        }
    
        _inventoryAllItems[item] -= totalRemoved;
        if(_inventoryAllItems[item] <= 0) _inventoryAllItems.Remove(item);
        
        ItemRedraw();
    
        return totalRemoved;
    }

    public InventorySlot DragAndDrop(bool alternateBehaviour = false)
    {
        if (!IsDragging)
        {
            return StartDrag(alternateBehaviour);
        }
        else
        {
            return EndDrag(alternateBehaviour);
        }
    }
    
    public bool InventoryItemWithAmountCheck(ItemSO item, int amount)
    {
        return _inventoryAllItems.ContainsKey(item) && _inventoryAllItems[item] >= amount;
    }
    public bool InventoryItemCheck(ItemSO item)
    {
        return _inventoryAllItems.ContainsKey(item);
    }

    private InventorySlot StartDrag(bool half = false)
    {
        if (_quickTransfer)
        {
            if (HoveredSlot != null)
            {
                
                if (_armorSlots.Contains(HoveredSlot))
                {
                    AddItem(HoveredSlot.GetItem(), HoveredSlot.GetItemAmount(), _allSlots);

                    if(HoveredSlot.wearType != WearableType.NonWearable)
                        PlayerEquipController.Instance.UnequipItem(HoveredSlot.GetItem());
                    else
                        throw new WarningException(
                            "Inventory Controller: you try take off wearable item from non wearable slot, wtf are you doing!?");
                
                    HoveredSlot.ClearSlot();
                
                    OnInventoryChanged?.Invoke(this);
                }
                else if (_hotbarSlots.Contains(HoveredSlot) || _inventorySlots.Contains(HoveredSlot))
                {
                    WearableType type = HoveredSlot.GetItem().itemWearType;
                    if (type != WearableType.NonWearable)
                    {
                        foreach (InventorySlot armorSlot in _armorSlots)
                        {
                            if(!armorSlot.HasItem() && armorSlot.wearType == type)
                            {
                                SlotSetter(HoveredSlot.GetItem(), armorSlot, 1);
                                HoveredSlot.ClearSlot();
                                return HoveredSlot;
                            }
                        }
                    }
                        
                    if (_hotbarSlots.Contains(HoveredSlot))
                    {
                        AddItem(HoveredSlot.GetItem(), HoveredSlot.GetItemAmount(), _inventorySlots);
                    }
                    else
                    {
                        AddItem(HoveredSlot.GetItem(), HoveredSlot.GetItemAmount(), _hotbarSlots);
                    }
                }
                
                HoveredSlot.ClearSlot();
            }
            
            return HoveredSlot;
        }
        
        if (HoveredSlot != null && HoveredSlot.HasItem())
        {
            _draggedSlot = HoveredSlot;
            IsDragging = true;

            if (half)
            {
                int draggedSlotAmount = _draggedSlot.GetItemAmount();
                int halfDragAmount = Math.Max(1, Mathf.CeilToInt(draggedSlotAmount / 2));
                
                dragSlot.SetItem(HoveredSlot.GetItem(), halfDragAmount);
                
                if(draggedSlotAmount > halfDragAmount)
                    _draggedSlot.RemoveAmount(halfDragAmount);
                else
                {
                    _draggedSlot.ClearSlot();
                }
            }
            else
            {
                dragSlot.SetItem(_draggedSlot.GetItem(), _draggedSlot.GetItemAmount());
                
                _draggedSlot.ClearSlot();
                
            }
            
            _inventoryAllItems[dragSlot.GetItem()] -= dragSlot.GetItemAmount();
            if(_inventoryAllItems[dragSlot.GetItem()] == 0) _inventoryAllItems.Remove(dragSlot.GetItem());
            
            if(HoveredSlot.wearType != WearableType.NonWearable)
                PlayerEquipController.Instance.UnequipItem(dragSlot.GetItem());
        }
        else
        {
            IsDragging = false;
        }
        
        OnInventoryChanged?.Invoke(this);
        ItemRedraw();
        
        return HoveredSlot;
    }

    [CanBeNull]
    private InventorySlot EndDrag(bool alternateBehaviour)
    {
        HandleDrop(from: dragSlot, to: HoveredSlot, alternateBehaviour);

        if(!dragSlot.HasItem())
        {
            _draggedSlot = null;
            IsDragging = false;
            
        }
        
        OnInventoryChanged?.Invoke(this);
        
        return null;
    }

    private bool SlotSetter(ItemSO item, InventorySlot to, int amount)
    {
        to.SetItem(item, amount);
        
        if(to.GetItemAmount() <= 0) to.ClearSlot();
        
        if (to != null && to.HasItem())
        {
            WearableType typeWear = to.GetItem().itemWearType;
            if (typeWear != WearableType.NonWearable && typeWear == to.wearType)
            {
                PlayerEquipController.Instance.EquipItem(to.GetItem());
            }
        }
        
        return true;
    }

    private void HandleDrop(InventorySlot from, InventorySlot to, bool alternateBehaviour = false)
    {
        if (to == null) {
            InnerDropItem(from, !alternateBehaviour ? from.GetItemAmount() : 1);
            return;
        }

        if (to.specialType != ItemTypeEnum.DEFAULT && from.GetItem().itemType != to.specialType) return;

        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetItemAmount();

            if (space > 0)
            {
                int move = 1;
                if(!alternateBehaviour) move = Mathf.Min(space, from.GetItemAmount());

                SlotSetter(to.GetItem(), to, to.GetItemAmount() + move);
                SlotSetter(from.GetItem(), from, from.GetItemAmount() - move);
                
                if (from.GetItemAmount() <= 0)
                {
                    from.ClearSlot();
                }

                if(_allSlots.Contains(to))
                {
                    if (_inventoryAllItems.ContainsKey(to.GetItem())) _inventoryAllItems[to.GetItem()] += move;
                    else _inventoryAllItems.Add(to.GetItem(), move);
                }
                
                return;
            }
        }

        if (to.HasItem())
        {
            ItemWithAmount tempItem = new ItemWithAmount(to.GetItem(), to.GetItemAmount());
            
            SlotSetter(from.GetItem(), to, from.GetItemAmount());
            SlotSetter(tempItem.Item, from, tempItem.Amount);

            if (_allSlots.Contains(to))
            {
                if (_inventoryAllItems.ContainsKey(to.GetItem())) _inventoryAllItems[to.GetItem()] += to.GetItemAmount();
                else _inventoryAllItems.Add(to.GetItem(), to.GetItemAmount());

                _inventoryAllItems[from.GetItem()] -= from.GetItemAmount();
                if (_inventoryAllItems[from.GetItem()] == 0) _inventoryAllItems.Remove(from.GetItem());
            }

            return;
        }

        if(alternateBehaviour)
        {
            SlotSetter(from.GetItem(), to, 1);
            SlotSetter(from.GetItem(), from, from.GetItemAmount()-1);
        }
        else
        {
            SlotSetter(from.GetItem(), to, from.GetItemAmount());
            from.ClearSlot();
        }

        if ( _allSlots.Contains(to))
        {
            if (_inventoryAllItems.ContainsKey(to.GetItem())) _inventoryAllItems[to.GetItem()] += to.GetItemAmount();
            else _inventoryAllItems.Add(to.GetItem(), to.GetItemAmount());
        }

        ItemRedraw();
    }

    private void UpdateDragItemPosition()
    {
        if (IsDragging)
        {
            dragSlot.transform.position = Input.mousePosition + new Vector3(dragSlotOffset.x, dragSlotOffset.y, 0);
        }
    }

    private void RebuildInventoryCache()
    {
        _inventoryAllItems.Clear();
        foreach (InventorySlot slot in _allSlots)
        {
            if (slot.HasItem())
            {
                ItemSO item = slot.GetItem();
                int amount = slot.GetItemAmount();
            
                if (_inventoryAllItems.ContainsKey(item))
                    _inventoryAllItems[item] += amount;
                else
                    _inventoryAllItems.Add(item, amount);
            }
        }
        
        OnInventoryChanged?.Invoke(this);
    }

    public void InventoryClosedHandler()
    {
        if (IsDragging)
        {
            AddItem(dragSlot.GetItem(), dragSlot.GetItemAmount());
            
            _draggedSlot = null;
            IsDragging = false;
            
            dragSlot.ClearSlot();
        }

        _quickTransfer = false;
    }

    public void InnerDropItem(InventorySlot fromDrop, int amount)
    {
        ItemSO droppedItem = fromDrop.GetItem();
                
        fromDrop.RemoveAmount(amount);

        WorldSpawner.Instance.PlayerDrop(droppedItem.itemWorldPrefab, PlayerStats.Instance.DropForce, false, amount);
    }

    public GameObject DropItem(bool dropFromInventory)
    {
        if(!dropFromInventory){
            InventorySlot selectedSlot = _hotbarSlots[SelectedHotbarSlot];

            if (selectedSlot.HasItem())
            {
                ItemSO droppedItem = selectedSlot.GetItem();
                
                selectedSlot.RemoveAmount(1);

                ItemRedraw();

                OnInventoryChanged?.Invoke(this);

                return droppedItem.itemWorldPrefab;
            }
        }
        else
        {
            if (HasHoveredSlot() && HoveredSlot.HasItem())
            {
                ItemSO droppedItem = HoveredSlot.GetItem();
                
                HoveredSlot.RemoveAmount(1);
                
                ItemRedraw();

                OnInventoryChanged?.Invoke(this);
                
                return droppedItem.itemWorldPrefab;
            }
        }

        return null;
    }

    public void SetQuickTransfer()
    {
        _quickTransfer = !_quickTransfer;
        
        ItemRedraw();
    }

    public void HotbarSelectorUpdate(int value)
    {
        value = value < 0 ? _hotbarSlots.Capacity-1 : value > _hotbarSlots.Capacity-1 ? 0 : value;
        SelectedHotbarSlot = value;
        
        ItemRedraw();
    }

    public void ItemRedraw()
    {
        InventorySlot currentSlot = _hotbarSlots[SelectedHotbarSlot];

        PlayerEquipController.Instance.TakeInHand(currentSlot.HasItem() ? currentSlot.GetItem() : null);
    }
    
    public void HoverSlot(InventorySlot slot)
    {
        HoveredSlot = slot;
    }

    public void UnhoverSlot()
    {
        HoveredSlot = null;
    }
    
    public bool HasHoveredSlot()
    {
        return HoveredSlot != null;
    }
}
