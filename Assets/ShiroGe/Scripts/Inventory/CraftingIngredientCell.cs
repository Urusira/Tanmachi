using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CraftingIngredientCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event System.Action<CraftingIngredientCell> OnHoverStart;
    public event System.Action<CraftingIngredientCell> OnHoverEnd;
    
    [SerializeField] private  Image ingredientImage;
    [SerializeField] private  TextMeshProUGUI ingredientAmount;
    
    public string IngredientName { get; private set; }
    public string IngredientDescription  { get; private set; }

    public void SetFields(Sprite ingredientSprite, string ingredientTitle, string ingredientDesc, int amount)
    {
        ingredientImage.sprite = ingredientSprite;
        IngredientName = ingredientTitle;
        IngredientDescription = ingredientDesc;
        ingredientAmount.text = amount.ToString();
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