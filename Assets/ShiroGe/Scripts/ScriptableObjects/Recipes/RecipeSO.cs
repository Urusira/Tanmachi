using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ingredient
{
    public ItemSO item;
    public int amount;
}

[CreateAssetMenu(fileName = "Recipe", menuName = "NewRecipe")]
public class RecipeSO : ScriptableObject
{
    public ItemSO result;
    public List<Ingredient> ingredients;
    public int resultAmount;
}
