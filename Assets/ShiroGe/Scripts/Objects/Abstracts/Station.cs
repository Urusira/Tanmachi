using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.UI;
using UnityEngine;

namespace ShiroGe.Scripts.Objects
{
    public abstract class Station : Interactable
    {
        public List<RecipeSO> recipes = new List<RecipeSO>();
        
        protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
        {
            InventoryUiManager.Instance.ShowInventory(recipes);
            
            return PlayerActionsState.Default;
        }

        public override string ShowHint()
        {
            base.ShowHint();
            return $"{name}\nF для взаимодействия";
        }
    }
}