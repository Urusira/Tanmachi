using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingPanelController : MonoBehaviour
{
    public event System.Action<CraftingRecipeCard> OnCraftClick;
    public event System.Action<CraftingIngredientCell> OnIngredientHoverStart;
    public event System.Action<CraftingIngredientCell> OnIngredientHoverEnd;
    
    private List<RecipeSO> _recipes;
    private List<CraftingRecipeCard> _recipesCards = new List<CraftingRecipeCard>();

    [SerializeField] private GameObject recipeCardPrefab;
    [SerializeField] private GameObject scrollContent;

    private void OnDestroy()
    {
        ClearRecipes();
        Destroy(this);
    }

    public void SetNewRecipesList(List<RecipeSO> recipes)
    {
        if (_recipes == recipes) return;
        
        _recipes = recipes;
        
        ClearRecipes();

        foreach (RecipeSO recipe in recipes)
        {
            AddRecipe(recipe);
        }
    }

    public void AddRecipe(RecipeSO recipe)
    {
        CraftingRecipeCard newRecipe = Instantiate(recipeCardPrefab, scrollContent.transform).GetComponent<CraftingRecipeCard>();
        
        newRecipe.SetRecipe(recipe);
        
        newRecipe.OnCraftClicked += OnCraftButtonClickHandler;
        newRecipe.OnIngredientHoverStart += OnIngredientHoverStartHandler;
        newRecipe.OnIngredientHoverEnd += OnIngredientHoverEndHandler;
        
        _recipesCards.Add(newRecipe);
    }
    
    private void ClearRecipes()
    {
        foreach (Transform child in scrollContent.transform)
        {
            CraftingRecipeCard recipe = child.GetComponent<CraftingRecipeCard>();
            
            _recipesCards.Remove(recipe);
            
            recipe.OnCraftClicked -= OnCraftButtonClickHandler;
            recipe.OnIngredientHoverStart -= OnIngredientHoverStartHandler;
            recipe.OnIngredientHoverEnd -= OnIngredientHoverEndHandler;
            
            Destroy(child.gameObject);
        }
    }

    public void BlockRecipe(RecipeSO recipe)
    {
        foreach (CraftingRecipeCard recipeCard in _recipesCards)
        {
            if (recipeCard.Recipe == recipe)
            {
                recipeCard.CraftButtonLock();
            }
        }
    }
    
    public void UnblockRecipe(RecipeSO recipe)
    {
        foreach (CraftingRecipeCard recipeCard in _recipesCards)
        {
            if (recipeCard.Recipe == recipe)
            {
                recipeCard.CraftButtonUnlock();
            }
        }
    }

    private void OnIngredientHoverEndHandler(CraftingIngredientCell cell)
    {
        OnIngredientHoverEnd?.Invoke(cell);
    }

    private void OnIngredientHoverStartHandler(CraftingIngredientCell cell)
    {
        OnIngredientHoverStart?.Invoke(cell);
    }

    private void OnCraftButtonClickHandler(CraftingRecipeCard recipeCard)
    {
        OnCraftClick?.Invoke(recipeCard);
    }

    public void UpdateCraftsAllow(Func<ItemSO, int, bool> inventoryCheckFunction)
    {
        if(_recipes != null && _recipesCards.Count > 0)
        foreach (RecipeSO recipe in _recipes)
        {
            bool available = true;
                    
            List<Ingredient> ingredients = recipe.ingredients;
            foreach (Ingredient ingredient in ingredients)
            {
                if (!inventoryCheckFunction(ingredient.item, ingredient.amount))
                {
                    available = false;
                    break;
                }
            }

            if (!available)
            {
                BlockRecipe(recipe);
            }
            else
            {
                UnblockRecipe(recipe);
            }
        }
    }
}