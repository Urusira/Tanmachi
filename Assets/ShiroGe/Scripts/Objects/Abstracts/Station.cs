using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.UI;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public abstract class Station : Interactable
    {
        public string stationName;
        
        public List<RecipeSO> recipes = new List<RecipeSO>();
        
        public override PlayerActionsState Interact()
        {
            InventoryUiManager.Instance.ShowInventory(recipes);
            
            return PlayerActionsState.Default;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{stationName}\nF для взаимодействия";
        }
    }
}