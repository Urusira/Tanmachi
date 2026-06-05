using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace ShiroGe.Scripts.Tavern
{
    public class TavernTablesManager : MonoBehaviour
    {
        public static TavernTablesManager Instance { get; private set; }
        
        // Словарь, где ключ - стол, а значение - свободные места
        private List<TavernTable> _tavernTables =  new List<TavernTable>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void RegisterTable(TavernTable table)
        {
            _tavernTables.Add(table);
        }

        public void UnregisterTable(TavernTable table)
        {
            _tavernTables.Remove(table);
        }

        public TavernTable GetAvailablePlace(int reqAmo, bool completlyFreePlace = false)
        {
            TavernTable availablePlace;
            if (completlyFreePlace)
                availablePlace = _tavernTables.First(it => it.GetComponent<TavernTable>().AmountOccupedPlaces == 0);
            else
                availablePlace = _tavernTables.First(it => it.GetComponent<TavernTable>().AmountAvailablePlaces >= reqAmo);

            return availablePlace;
        }
    }
}