using System;
using ShiroGe.Scripts;
using UnityEngine;

namespace ShiroGe.Player.NPC
{
    public class Dialog : MonoBehaviour, Interactable
    {
        [SerializeField] private new string name = String.Empty;
       
        public void Interact()
        {
            DialogManager.Instance.StartDialog(name, GetInstanceID().ToString());
        }
    
        public void ShowHint()
        {
        
        }

        public void HideHint()
        {
        
        }
    }
}