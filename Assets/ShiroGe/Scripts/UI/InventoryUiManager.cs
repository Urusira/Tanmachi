using System;
using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.Tavern;
using ShiroGe.Scripts.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShiroGe.Scripts.UI
{
    
    [DefaultExecutionOrder(3)]
    public class InventoryUiManager : MonoBehaviour
    {
        public static InventoryUiManager Instance { get; private set; }
        
        [SerializeField] private GameObject inventoryObj;
        [SerializeField] private GameObject inventorySlotsObj;
        [SerializeField] private GameObject armorSlotsObj;
        [SerializeField] private GameObject ratingObj;
        [SerializeField] private GameObject cashObj;
        [SerializeField] private GameObject timeObj;
        [SerializeField] private GameObject hotbarSlotsObj;
        [SerializeField] private GameObject playerObj;

        [SerializeField] private GameObject craftingMenuObj;

        [SerializeField] private GameObject selectorBorderObj;
        
        [SerializeField] private InventoryDescriptionPanel descriptionPanel;
        
        [SerializeField] private Vector3 floatDescriptionOffset = new Vector3(15, -15, 0);

        private PlayerController _playerController;
        private RectTransform _inventorySlotsRectTransform;
        private RectTransform _inventoryHotbarRectTransform;
        private RectTransform _inventoryCraftRectTransform;
        
        private TextMeshProUGUI _cashText;
        private Slider _ratingSlider;
        private TextMeshProUGUI _timeText;

        private CraftingPanelController _craftingPanel;
        
        private Rect _inventorySlotsStdRect;
        
        private string _descriptionPanelTitle = "";
        private string _descriptionPanelDesc = "";
        
        public bool IsOpen { get; private set; } = false;
        public bool IsCrafting { get; private set; } = false;

        private bool _needDescription = false;
        
        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            //DontDestroyOnLoad(gameObject);

            Instance = this;
            
            inventoryObj.SetActive(false);
            
            _playerController = playerObj.GetComponent<PlayerController>();
            
            foreach (InventorySlot slot in InventoryManager.Instance._allSlots)
            {
                slot.OnHoverStart += HoverSlot;
                slot.OnHoverEnd += UnhoverSlot;
            }
            
            descriptionPanel.Hide();
            craftingMenuObj.SetActive(false);
            
            _inventorySlotsRectTransform = inventorySlotsObj.GetComponent<RectTransform>();
            _inventorySlotsStdRect = _inventorySlotsRectTransform.rect;
            
            _inventoryHotbarRectTransform = hotbarSlotsObj.GetComponent<RectTransform>();
            
            _inventoryCraftRectTransform = craftingMenuObj.GetComponent<RectTransform>();
            
            _craftingPanel = craftingMenuObj.GetComponent<CraftingPanelController>();
            
            _cashText = cashObj.GetComponent<TextMeshProUGUI>();
            _timeText = timeObj.GetComponent<TextMeshProUGUI>();
            _ratingSlider = ratingObj.GetComponent<Slider>();
            
            playerObj.GetComponent<CashManager>().OnCashChanged += OnCashChangedHandler;

            TimeManager.Instance.OnTimeTick += OnTimeTickHandler;

            TavernReputationManager.Instance.OnReputationChange += OnReputationChangeHandler;
            _ratingSlider.minValue = TavernReputationManager.Instance.MinReputation;
            _ratingSlider.maxValue = TavernReputationManager.Instance.MaxReputation;
            _ratingSlider.value = TavernReputationManager.Instance.CurrentReputation;
                
            InventoryManager.Instance.OnInventoryChanged += OnInventoryChangedHandler;
        }

        private void OnReputationChangeHandler(float newRating)
        {
            _ratingSlider.value = newRating;
        }

        private void OnTimeTickHandler(float _)
        {
            int[] time = TimeManager.Instance.Get24FormattedTime();
            
            _timeText.text = "День " + time[0] + ", " + time[1] + ":" + (time[2]/10 <= 0 ? "0"+time[2] :  time[2]) + "\n" + TimeManager.Instance.NamedCurrentDayPhase;
        }

        private void OnCashChangedHandler(float newValue)
        {
            _cashText.text = newValue.ToString();
        }

        private void Update()
        {
            UpdateDescriptionPanel();
        }

        public void ShowInventory(List<RecipeSO> craftList = null)
        {
            GuiManager.Instance.HideGui();
            GuiManager.Instance.UnlockMouse();
            _playerController.LockControl();
            
            inventoryObj.SetActive(true);
            IsOpen = true;

            if (craftList != null)
            {
                IsCrafting = true;
                craftingMenuObj.SetActive(true);
                
                _inventorySlotsRectTransform.position += new Vector3(_inventoryCraftRectTransform.rect.width/2, 0, 0);
                _inventoryHotbarRectTransform.position += new Vector3(_inventoryCraftRectTransform.rect.width/2, 0, 0);
                
                _craftingPanel.SetNewRecipesList(craftList);

                _craftingPanel.OnCraftClick += OnCraftClickHandler;
                _craftingPanel.OnIngredientHoverStart += OnIngredientHoverStartHandler;
                _craftingPanel.OnIngredientHoverEnd += OnIngredientHoverEndHandler;

                _craftingPanel.UpdateCraftsAllow(InventoryManager.Instance.InventoryItemWithAmountCheck);
            }
        }

        private void OnInventoryChangedHandler(InventoryManager _)
        {
            _craftingPanel.UpdateCraftsAllow(InventoryManager.Instance.InventoryItemWithAmountCheck);
        }

        public void HideInventory()
        {
            InventoryManager.Instance.InventoryClosedHandler();
            
            GuiManager.Instance.ShowGui();
            GuiManager.Instance.LockMouse();
            _playerController.UnlockControl();
            
            if (IsCrafting)
            {
                IsCrafting = false;
                craftingMenuObj.SetActive(false);
                _inventorySlotsRectTransform.position -= new Vector3(_inventoryCraftRectTransform.rect.width/2, 0, 0);
                _inventoryHotbarRectTransform.position -= new Vector3(_inventoryCraftRectTransform.rect.width/2, 0, 0);
                
                _craftingPanel.OnCraftClick -= OnCraftClickHandler;
                _craftingPanel.OnIngredientHoverStart -= OnIngredientHoverStartHandler;
                _craftingPanel.OnIngredientHoverEnd -= OnIngredientHoverEndHandler;
                
            }
        
            descriptionPanel.Hide();
            _needDescription = false;
            
            inventoryObj.SetActive(false);
            IsOpen = false;
        }

        public void LeftClick()
        {
            if(!IsOpen) return;
            
            InventorySlot slot = InventoryManager.Instance.DragAndDrop();
            if (slot != null)
            {
                HoverSlot(slot);
            }
        }

        public void RightClick()
        {
            if(!IsOpen) return;
            
            InventorySlot slot = InventoryManager.Instance.DragAndDrop(half: true);
            
            if (slot != null)
            {
                HoverSlot(slot);
            }
        }

        public void SetQuickTransfer()
        {
            if(!IsOpen) return;

            InventoryManager.Instance.SetQuickTransfer();
        }

        public void HotbarSelect(int value)
        {
            InventoryManager.Instance.HotbarSelectorUpdate(value);
            selectorBorderObj.transform.position = new Vector3(
                InventoryManager.Instance._hotbarSlots[InventoryManager.Instance.SelectedHotbarSlot].transform.position.x, 
                selectorBorderObj.transform.position.y, 
                selectorBorderObj.transform.position.z
                );
        }

        public void HotbarHide()
        {
            hotbarSlotsObj.SetActive(false);
        }

        public void HotbarShow()
        {
            hotbarSlotsObj.SetActive(true);
        }
        
        private void UpdateDescriptionPanel()
        {
            if(!IsOpen) return;

            if (_needDescription && !InventoryManager.Instance.IsDragging)
            {
                Vector3 mPos = Input.mousePosition;
            
                descriptionPanel.UpdateDescritpionPanelPosition(mPos + new Vector3(
                    descriptionPanel.ObjectTransform.rect.width / 2 + floatDescriptionOffset.x, 
                    descriptionPanel.ObjectTransform.rect.height / 2 + floatDescriptionOffset.y, 
                    0)
                );
                
                descriptionPanel.Show(_descriptionPanelTitle, _descriptionPanelDesc);
            }
            else
            {
                descriptionPanel.Hide();
            }
        }
        
        public void NextItem()
        {
            if(IsOpen) return;

            HotbarSelect(InventoryManager.Instance.SelectedHotbarSlot+1);
        }
        
        public void PreviousItem()
        {
            if(IsOpen) return;

            HotbarSelect(InventoryManager.Instance.SelectedHotbarSlot-1);
        }

        private void HoverSlot(InventorySlot slot)
        {
            if(!IsOpen) return;

            InventoryManager.Instance.HoverSlot(slot);
            
            if(slot.HasItem())
            {
                _needDescription = true;
                
                _descriptionPanelTitle = slot.GetItem().itemName;
                _descriptionPanelDesc = slot.GetItem().itemDescription;
            }
            else
            {
                _needDescription = false;
                
                _descriptionPanelTitle = "";
                _descriptionPanelDesc = "";
            }
        }

        private void UnhoverSlot(InventorySlot _)
        {
            if(!IsOpen) return;

            InventoryManager.Instance.UnhoverSlot();
            
            _needDescription = false;
            
            _descriptionPanelTitle = "";
            _descriptionPanelDesc = "";
        }
        
        private void OnIngredientHoverStartHandler(CraftingIngredientCell cell)
        {
            if(!IsOpen) return;
            
            _needDescription = true;
            
            _descriptionPanelTitle = cell.IngredientName;
            _descriptionPanelDesc = cell.IngredientDescription;
        }
        
        private void OnIngredientHoverEndHandler(CraftingIngredientCell _)
        {
            if(!IsOpen) return;
            
            _needDescription = false;
            
            _descriptionPanelTitle = "";
            _descriptionPanelDesc = "";
        }

        private void OnCraftClickHandler(CraftingRecipeCard recipeCard)
        {
            if(!IsOpen) return;

            RecipeSO recipe = recipeCard.Recipe;
            
            InventoryManager.Instance.AddItem(recipe.result, recipe.resultAmount);

            foreach (Ingredient ingredient in recipe.ingredients)
            {
                InventoryManager.Instance.RemoveItem(ingredient.item, ingredient.amount);
            }
        }
    }
}