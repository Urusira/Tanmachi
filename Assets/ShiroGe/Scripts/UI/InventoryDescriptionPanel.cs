using System;
using TMPro;
using UnityEngine;

namespace ShiroGe.Scripts.UI
{
    public class InventoryDescriptionPanel : MonoBehaviour
    {
        [SerializeField] private GameObject NameTextObj;
        [SerializeField] private GameObject DescriptionTextObj;
        
        public RectTransform ObjectTransform { get; private set; }
        
        private TextMeshProUGUI NameText;
        private TextMeshProUGUI DescriptionText;
        
        private void Start()
        {
            NameText = NameTextObj.GetComponent<TextMeshProUGUI>();
            DescriptionText = DescriptionTextObj.GetComponent<TextMeshProUGUI>();
            
            ObjectTransform = GetComponent<RectTransform>();
        }

        private void SetNameText(string text)
        {
            NameText.text = text;
        }

        private void SetDescriptionText(string text)
        {
            DescriptionText.text = text;
        }

        public void UpdateDescritpionPanelPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void Hide()
        {
            SetNameText("");
            SetDescriptionText("");
            gameObject.SetActive(false);
        }
        
        public void Show(string title, string description)
        {
            gameObject.SetActive(true);
            SetNameText(title);
            SetDescriptionText(description);
        }
    }
}