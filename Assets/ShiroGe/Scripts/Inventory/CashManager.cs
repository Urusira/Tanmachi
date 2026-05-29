using UnityEngine;

namespace ShiroGe.Scripts.Inventory
{
    public class CashManager : MonoBehaviour
    {
        public event System.Action<float> OnCashChanged;
        
        public float CashAmount { get; private set; } = 0f;

        public void addCash(float cashAmount)
        {
            this.CashAmount += cashAmount;
            
            OnCashChanged?.Invoke(cashAmount);
        }

        public void removeCash(float cashAmount)
        {
            this.CashAmount -= cashAmount;
            
            OnCashChanged?.Invoke(cashAmount);
        }
    }
}