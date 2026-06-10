using UnityEngine;

namespace ShiroGe.Scripts.Inventory
{
    public class CashManager : MonoBehaviour
    {
        public event System.Action<float> OnCashChanged;
        
        public int CashAmount { get; private set; } = 0;

        public bool AddCash(int cashAmount)
        {
            this.CashAmount += cashAmount;
            
            OnCashChanged?.Invoke(this.CashAmount);
            
            return true;
        }

        public bool RemoveCash(int cashAmount)
        {
            this.CashAmount -= cashAmount;
            
            OnCashChanged?.Invoke(this.CashAmount);

            return true;
        }

        public bool CanRemoveCash(float cashAmount)
        {
            return cashAmount <= this.CashAmount;
        }
    }
}