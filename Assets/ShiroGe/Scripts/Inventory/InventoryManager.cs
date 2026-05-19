using System;
using System.Collections.Generic;
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
    
    [SerializeField] private GameObject hotbarObj;
    [SerializeField] private GameObject inventorySlotParent;

    [SerializeField] private InventoryDragSlot dragSlot;

    [SerializeField] private Vector2 dragSlotOffset = new Vector2(90, -90);
    
    public List<InventorySlot> _inventorySlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _hotbarSlots { get; private set; } = new List<InventorySlot>();
    public List<InventorySlot> _allSlots { get; private set; } = new List<InventorySlot>();
    
    private InventorySlot _draggedSlot = null;

    public int SelectedHotbarSlot { get; private set; } = 0;
    
    private bool _isDragging = false;
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
    }

    private void Awake()
    {
        _inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<InventorySlot>());
        _hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<InventorySlot>());
        
        _allSlots.AddRange(_hotbarSlots);
        _allSlots.AddRange(_inventorySlots);
    }

    private void Update()
    {
        if(_isDragging)
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
                        return remaining;
                }
            }
        }

        foreach (InventorySlot slot in targetInventoryZone)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;
                
                if(remaining <= 0)
                    return remaining;
            }
        }

        if (remaining > 0)
        {
            Debug.Log($"Inventory Full, could not add {remaining} of {itemToAdd.itemName}");
        }

        return remaining;
    }

    public void DragAndDrop(bool half = false)
    {
        if (!_isDragging)
        {
            StartDrag(half);
        }
        else
        {
            EndDrag();
        }
    }

    public bool HotbarItemCheck(ItemSO item)
    {
        foreach (InventorySlot slot in _hotbarSlots)
        {
            if (slot.GetItem() == item) return true;
        }

        return false;
    }
    
    public bool InventoryItemCheck(ItemSO item)
    {
        foreach (InventorySlot slot in _inventorySlots)
        {
            if (slot.GetItem() == item) return true;
        }

        return false;
    }
    
    public bool AllInventoryItemCheck(ItemSO item)
    {
        return HotbarItemCheck(item) || InventoryItemCheck(item);
    }

    private void StartDrag(bool half = false)
    {
        InventorySlot hovered = GetHoveredSlot();

        if (_quickTransfer)
        {
            ItemSO tempItem = hovered.GetItem();
            int tempAmount = hovered.GetItemAmount();
                
            hovered.ClearSlot();
            
            if (_hotbarSlots.Contains(hovered))
            {
                AddItem(tempItem, tempAmount, _inventorySlots);
            }
            else
            {
                AddItem(tempItem, tempAmount, _hotbarSlots);
            }
            
            return;
        }
        
        if (hovered != null && hovered.HasItem())
        {
            _draggedSlot = hovered;
            _isDragging = true;

            if (half)
            {
                int draggedSlotAmount = _draggedSlot.GetItemAmount();
                int halfDragAmount = Math.Max(1, Mathf.CeilToInt(draggedSlotAmount / 2));
                
                dragSlot.SetItem(hovered.GetItem(), halfDragAmount);
                
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
        }
        else
        {
            _isDragging = false;
        }
    }

    private void EndDrag()
    {
        InventorySlot hovered = GetHoveredSlot();

        HandleDrop(from: dragSlot, to: hovered);

        if(!dragSlot.HasItem())
        {
            _draggedSlot = null;
            _isDragging = false;
        }
    }

    private InventorySlot GetHoveredSlot()
    {
        foreach (InventorySlot slot in _allSlots)
        {
            if (slot.hovering)
            {
                return slot;
            }
        }
        return null;
    }

    private void HandleDrop(InventoryDragSlot from, InventorySlot to)
    {
        if (to == null) return;

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

                return;
            }
        }

        if (to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetItemAmount();
            
            to.SetItem(from.GetItem(), from.GetItemAmount());
            from.SetItem(tempItem, tempAmount);

            return;
        }
        
        to.SetItem(from.GetItem(), from.GetItemAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if (_isDragging)
        {
            dragSlot.transform.position = Input.mousePosition + new Vector3(dragSlotOffset.x, dragSlotOffset.y, 0);
        }
    }

    public void InventoryClosedHandler()
    {
        if(_isDragging && _draggedSlot != null)
            EndDrag();
        
        _quickTransfer = false;
    }

    public void DropItem()
    {
        InventorySlot selectedSlot = _hotbarSlots[SelectedHotbarSlot];
        WorldSpawner.Instance.PlayerDrop(selectedSlot.GetItem().itemPrefab);
        
        selectedSlot.RemoveAmount(1);
    }

    public void SetQuickTransfer()
    {
        _quickTransfer = !_quickTransfer;
    }

    public void HotbarSelectorUpdate(int value)
    {
        value = value < 0 ? _hotbarSlots.Capacity-1 : value > _hotbarSlots.Capacity-1 ? 0 : value;
        SelectedHotbarSlot = value;
    }
}
