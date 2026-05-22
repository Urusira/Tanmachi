using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeCard : MonoBehaviour
{
    public event System.Action<CraftingRecipeCard> OnCraftClicked;
    public event System.Action<CraftingIngredientCell> OnIngredientHoverStart;
    public event System.Action<CraftingIngredientCell> OnIngredientHoverEnd;
    
    public RecipeSO Recipe { get; private set; }
    
    [SerializeField] private GameObject recipeIngredientPrefab;
    [SerializeField] private GameObject scrollContent;
    
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultTitle;
    
    [SerializeField] private Button craftButton;

    private List<Ingredient> _ingredients;
    
    private void OnDestroy()
    {
        ClearIngredients();
        Destroy(this);
    }

    public void SetRecipe(RecipeSO recipe)
    {
        Recipe = recipe;
        
        resultIcon.sprite = recipe.result.icon;
        resultTitle.text = recipe.result.itemName;
        
        SetNewIngredientsList(Recipe.ingredients);

    }
    
    public void SetNewIngredientsList(List<Ingredient> ingredients)
    {
        _ingredients = ingredients;
        
        ClearIngredients();
        
        foreach (Ingredient ingredient in ingredients)
        {
            AddIngredient(ingredient);
        }
    }
    
    public void AddIngredient(Ingredient ingredient)
    {
        GameObject instaIngredient = Instantiate(recipeIngredientPrefab, scrollContent.transform);
        
        CraftingIngredientCell cell = instaIngredient.GetComponent<CraftingIngredientCell>();
        cell.SetFields(
            ingredientSprite: ingredient.item.icon,  
            ingredientTitle: ingredient.item.itemName, 
            ingredientDesc: ingredient.item.itemDescription, 
            amount: ingredient.amount
            );
        
        cell.OnHoverStart += OnIngredientHoverStartHandler;
        cell.OnHoverEnd += OnIngredientHoverEndHandler;
    }
    
    private void ClearIngredients()
    {
        foreach (Transform child in scrollContent.transform)
        {
            CraftingIngredientCell cell = child.GetComponent<CraftingIngredientCell>();

            if(cell != null)
            {
                cell.OnHoverStart -= OnIngredientHoverStartHandler;
                cell.OnHoverEnd -= OnIngredientHoverEndHandler;
            }
            
            Destroy(child.gameObject);
        }
    }

    public void CraftButtonLock()
    {
        craftButton.interactable = false;
    }
    public void CraftButtonUnlock()
    {
        craftButton.interactable = true;
    }

    public void OnCraftClickHandler()
    {
        OnCraftClicked?.Invoke(this);
    }
    
    private void OnIngredientHoverStartHandler(CraftingIngredientCell cell)
    {
        OnIngredientHoverStart?.Invoke(cell);
    }

    private void OnIngredientHoverEndHandler(CraftingIngredientCell cell)
    {
        OnIngredientHoverEnd?.Invoke(cell);
    }
}