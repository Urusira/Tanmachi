using System;
using System.Collections.Generic;
using ShiroGe.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance  { get; private set; }
    
    public GameObject hotbarObj;
    public GameObject inventorySlotParent;

    public Image dragIcon;
    
    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();
    private List<InventorySlot> _hotbarSlots = new List<InventorySlot>();
    private List<InventorySlot> _allSlots = new List<InventorySlot>();
    
    private InventorySlot _draggedSlot = null;
    private bool _isDragging = false;

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
        if(_isDragging)
            UpdateDragItemPosition();
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

    public void DragAndDrop()
    {
        _isDragging = !_isDragging;
        
        if (_isDragging)
        {
            StartDrag();
        }
        else
        {
            EndDrag();
        }
    }

    private void StartDrag()
    {
        InventorySlot hovered = GetHoveredSlot();

        if (hovered != null && hovered.HasItem())
        {
            _draggedSlot = hovered;
            _isDragging = true;

            dragIcon.sprite = hovered.GetItem().icon;
            dragIcon.color = new Color(1, 1, 1, 0.5f);
            dragIcon.enabled = true;
        }
        else
        {
            _isDragging = false;
        }
    }

    private void EndDrag()
    {
        InventorySlot hovered = GetHoveredSlot();

        HandleDrop(_draggedSlot, hovered);
        
        dragIcon.enabled = false;

        _draggedSlot = null;
        _isDragging = false;
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

    private void HandleDrop(InventorySlot from, InventorySlot to)
    {
        if (from == to || to == null || from == null) return;

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
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void InventoryClosedHandler()
    {
        if(_isDragging && _draggedSlot != null)
            EndDrag();
    }
}
