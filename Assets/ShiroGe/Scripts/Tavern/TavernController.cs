using UnityEngine;

namespace ShiroGe.Scripts.Tavern
{
    public class TavernController : MonoBehaviour
    {
        public event System.Action OnTavernClose;
        public event System.Action OnTavernOpen;
        
        public static TavernController Instance { get; private set; }
        public bool TavernOpen { get; private set; } = true;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void CloseTavern()
        {
            TavernOpen = false;
            OnTavernClose?.Invoke();
        }

        public void OpenTavern()
        {
            TavernOpen = true;
            OnTavernOpen?.Invoke();
        }
    }
}