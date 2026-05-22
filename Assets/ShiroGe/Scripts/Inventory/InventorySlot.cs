using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event System.Action<InventorySlot> OnHoverStart;
    public event System.Action<InventorySlot> OnHoverEnd;
    
    private ItemSO _heldItem;
    private int _itemAmount;
    
    [SerializeField] private GameObject iconImageObj;
    [SerializeField] private GameObject amountTxtObj;
    private Image _iconImage;
    private TextMeshProUGUI _amountTxt;

    private void Awake()
    {
        _iconImage = iconImageObj.GetComponent<Image>();
        _amountTxt = amountTxtObj.GetComponent<TextMeshProUGUI>();
    }

    public ItemSO GetItem()
    {
        return _heldItem;
    }

    public int GetItemAmount()
    {
        return _itemAmount;
    }

    public void SetItem(ItemSO item, int amount = 1)
    {
        _heldItem = item;
        _itemAmount = amount;

        UpdateSlot();
    }

    private void UpdateSlot()
    {
        if (_heldItem != null)
        {
            _iconImage.enabled = true;
            _iconImage.sprite = _heldItem.icon;
            _amountTxt.text = _itemAmount.ToString();
        }
        else
        {
            _iconImage.enabled = false;
            _amountTxt.text = "";
        }
    }

    public int AddAmount(int amount)
    {
        _itemAmount += amount;
        UpdateSlot();
        return _itemAmount;
    }

    public int RemoveAmount(int amount)
    {
        _itemAmount -= amount;
        
        if (_itemAmount <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateSlot();
        }
        
        return _itemAmount;
    }

    public void ClearSlot() 
    {
        _heldItem = null;
        _itemAmount = 0;
        UpdateSlot();
    }

    public bool HasItem()
    {
        return _heldItem != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverStart?.Invoke(this);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverEnd?.Invoke(this);
    }
}
