using System;
using System.Collections.Generic;
using System.Linq;
using ShiroGe.Scripts.NPC;
using UnityEngine;

namespace ShiroGe.Scripts.Tavern
{
    public class TavernTablesManager : MonoBehaviour
    {
        public static TavernTablesManager Instance { get; private set; }
        
        private List<TavernTable> _tavernTables =  new List<TavernTable>();

        public int CompletlyFreeTables { get; private set; }
        public int FreePlaces { get; private set; }
        public int TotalPlaces { get; private set; }

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
            table.OnPlacesAvailabilityChanged += OnPlacesTakenHandler;
            table.OnPlacesReleased += OnPlacesReleasedHandler;
            
            TotalPlaces += table.AmountAvailablePlaces + table.AmountOccupedPlaces;
            FreePlaces += table.AmountAvailablePlaces;
            CompletlyFreeTables++;
        }

        public void UnregisterTable(TavernTable table)
        {
            _tavernTables.Remove(table);
            
            table.OnPlacesAvailabilityChanged -= OnPlacesTakenHandler;
            table.OnPlacesReleased -= OnPlacesReleasedHandler;
            
            TotalPlaces = Math.Max(TotalPlaces - (table.AmountAvailablePlaces + table.AmountOccupedPlaces), 0);
            FreePlaces = Math.Max(FreePlaces-table.AmountAvailablePlaces, 0);
            CompletlyFreeTables = table.AmountOccupedPlaces == 0 ? Math.Max(CompletlyFreeTables - 1, 0) : CompletlyFreeTables;
        }

        private void OnPlacesTakenHandler(TavernTable table)
        {
            if(table.AmountOccupedPlaces == 1) CompletlyFreeTables = Math.Max(CompletlyFreeTables - 1, 0);
            FreePlaces = Math.Max(FreePlaces - 1, 0);
            
        }

        private void OnPlacesReleasedHandler(TavernTable table)
        {
            FreePlaces = Math.Min(FreePlaces + 1, TotalPlaces);
            if(table.AmountOccupedPlaces == 0) 
                CompletlyFreeTables = Math.Min(CompletlyFreeTables + 1, _tavernTables.Count);
        }

        public TavernTable GetAvailableTable(int reqAmo, bool needCompletlyFreePlace = false)
        {
            TavernTable availablePlace;
            if (needCompletlyFreePlace)
                availablePlace = _tavernTables.FirstOrDefault(it =>
                {
                    return it.AmountOccupedPlaces == 0 && !it.TableFullReserved;
                });
            else
                availablePlace = _tavernTables.FirstOrDefault(it =>
                {
                    return it.AmountAvailablePlaces >= reqAmo && !it.TableFullReserved;
                });

            return availablePlace;
        }
    }
}