using System;
using System.Collections.Generic;
using System.Linq;
using ShiroGe.Scripts.Inventory;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Utils;
using UnityEngine;

namespace ShiroGe.Scripts.Tavern
{
    public class TavernMenuManager : MonoBehaviour
    {
        public static TavernMenuManager Instance { get; private set; }

        private MenuItem cheapestDish = null;
        
        [field: SerializeField] public List<MenuItem> tavernMenu { get; private set; }  = new List<MenuItem>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            cheapestDish = tavernMenu.OrderBy(item => item.price).FirstOrDefault();

            TavernReputationManager reputationManager;
        }

        /// <summary>
        /// Метод для получения набора случайных блюд, подходящих для нпс
        /// </summary>
        /// <param name="npc">НПС, для которого формируется меню</param>
        /// <returns>Список пар ключ-значение, где ключ - блюдо, а значение - цена</returns>
        public List<MenuItem> GetRandomDishes(NPCController npc)
        {
            List<MenuItem> menu = new List<MenuItem>();

            int cash = npc.GetComponent<CashManager>().CashAmount;

            foreach (MenuItem position in tavernMenu)
            {
                if(TavernReputationManager.Instance.CurrentReputation < position.requiredReputation || position.price > cash) continue;
                
                menu.Add(position);
            }
            
            return menu;
        }

        public ItemSO GetCheapestDish(int remainingCash)
        {
            return cheapestDish.price <= remainingCash ? cheapestDish.dish : null;
        }
    }
}