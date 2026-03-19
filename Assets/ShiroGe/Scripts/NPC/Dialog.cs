using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShiroGe.Scripts.NPC
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