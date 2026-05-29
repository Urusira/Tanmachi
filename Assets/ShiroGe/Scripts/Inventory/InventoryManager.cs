using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ShiroGe.CharacterController;
using ShiroGe.Scripts;
using ShiroGe.Scripts.Items;
using ShiroGe.Scripts.UI;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance  { get; private set; }

    public event System.Action<InventoryManager> OnInventoryChanged;
    
    [SerializeField] private GameObject hotbarObj;
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject armorSlotsObj;

    [SerializeField] private InventoryDragSlot dragSlot;

    [SerializeField] private Vector2 dragSlotOffset = new Vector2(90, -90);
    
    public List<InventorySlot> _inventorySlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _hotbarSlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _armorSlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _allSlots { get; private set; } = new List<InventorySlot>();
    
    public InventorySlot HoveredSlot { get; private set; }

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
        if (actuallyAdded > 0 && !_quickTransfer)
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
        
        ItemRedraw();
    
        return totalRemoved;
    }

    public InventorySlot DragAndDrop(bool half = false)
    {
        if (!IsDragging)
        {
            return StartDrag(half);
        }
        else
        {
            return EndDrag();
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
                ItemSO tempItem = HoveredSlot.GetItem();
                int tempAmount = HoveredSlot.GetItemAmount();


                if (_armorSlots.Contains(HoveredSlot))
                {
                    AddItem(tempItem, tempAmount, _allSlots);
                }
                else if (_hotbarSlots.Contains(HoveredSlot))
                {
                    AddItem(tempItem, tempAmount, _inventorySlots);
                }
                else
                {
                    AddItem(tempItem, tempAmount, _hotbarSlots);
                }

                if(HoveredSlot.specialType != ItemTypeEnum.DEFAULT)
                    PlayerEquipController.Instance.UnequipItem(HoveredSlot.GetItem());
                
                HoveredSlot.ClearSlot();
                
                OnInventoryChanged?.Invoke(this);
                return HoveredSlot;
            }
            else return null;
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
            
            if(HoveredSlot.specialType != ItemTypeEnum.DEFAULT)
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
    private InventorySlot EndDrag()
    {
        HandleDrop(from: dragSlot, to: HoveredSlot);

        if(!dragSlot.HasItem())
        {
            _draggedSlot = null;
            IsDragging = false;
            
        }
        
        OnInventoryChanged?.Invoke(this);

        
        if (HoveredSlot != null && HoveredSlot.HasItem())
        {
            ItemTypeEnum type = HoveredSlot.GetItem().itemType;
            if (type != ItemTypeEnum.DEFAULT && type == HoveredSlot.specialType)
            {
                PlayerEquipController.Instance.EquipItem(HoveredSlot.GetItem());
            }
            
            return HoveredSlot;
        }
        
        return null;
    }

    private void HandleDrop(InventoryDragSlot from, InventorySlot to)
    {
        if (to == null) return;

        if (to.specialType != ItemTypeEnum.DEFAULT && from.GetItem().itemType != to.specialType) return;

        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetItemAmount();

            if (space > 0)
            {
                int move = Mathf.Min(space, from.GetItemAmount());
                
                to.SetItem(to.GetItem(), to.GetItemAmount() + move);
                from.SetItem(from.GetItem(), from.GetItemAmount() - move);

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
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetItemAmount();
            
            to.SetItem(from.GetItem(), from.GetItemAmount());
            from.SetItem(tempItem, tempAmount);

            if (_allSlots.Contains(to))
            {
                if (_inventoryAllItems.ContainsKey(to.GetItem())) _inventoryAllItems[to.GetItem()] += to.GetItemAmount();
                else _inventoryAllItems.Add(to.GetItem(), to.GetItemAmount());

                _inventoryAllItems[from.GetItem()] -= from.GetItemAmount();
                if (_inventoryAllItems[from.GetItem()] == 0) _inventoryAllItems.Remove(from.GetItem());
            }

            return;
        }
        
        to.SetItem(from.GetItem(), from.GetItemAmount());
        from.ClearSlot();

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

    public void DropItem()
    {
        InventorySlot selectedSlot = _hotbarSlots[SelectedHotbarSlot];
        
        if(selectedSlot.HasItem())
        {
            WorldSpawner.Instance.PlayerDrop(selectedSlot.GetItem().itemPrefab);
        
            selectedSlot.RemoveAmount(1);
            
            ItemRedraw();
            
            OnInventoryChanged?.Invoke(this);
        }
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
