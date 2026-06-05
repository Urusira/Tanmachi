using System;
using System.Collections.Generic;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Tavern;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class TavernTable : MonoBehaviour
{
    [SerializeField] private SitPlace[] sitPlaces;
    
    [field: SerializeField] public int AmountOccupedPlaces { get; private set; }  = 0;
    [field: SerializeField] public int AmountAvailablePlaces { get; private set; }  = 0;

    private void Awake()
    {
        foreach (SitPlace place in sitPlaces)
        {
            place.OnPlaceReserved += OnPlaceReservedHandler;
            place.OnPlaceTaken += OnPlaceTakenHandler;
            place.OnPlaceVacated += OnPlaceVacatedHandler;
        }

        RecountAvailablePlaces();
        
        TavernTablesManager.Instance.RegisterTable(this);
    }

    private void OnPlaceTakenHandler(SitPlace place)
    {
        RecountAvailablePlaces();
    }
    
    private void OnPlaceReservedHandler(SitPlace place)
    {
        RecountAvailablePlaces();
    }
    
    private void OnPlaceVacatedHandler(SitPlace place)
    {
        RecountAvailablePlaces();
    }

    public List<SitPlace> GetAvailablePlaces(int reqAmo, bool completlyFreePlace = true)
    {
        if (AmountAvailablePlaces < reqAmo || (completlyFreePlace && AmountOccupedPlaces != 0)) return null;
        
        List<SitPlace> places = new List<SitPlace>();
        
        foreach (SitPlace place in sitPlaces)
        {
            if (place.Available)
            {
                places.Add(place);
            }
        }
        
        return places;
    }

    public SitPlace GetAvailablePlace(bool completlyFreePlace = false)
    {
        if (AmountAvailablePlaces <= 0 || (completlyFreePlace && AmountOccupedPlaces != 0)) return null;
        
        foreach (SitPlace place in sitPlaces)
        {
            if (place.Available)
            {
                return place;
            }
        }
        
        return null;
    }
    
    private void RecountAvailablePlaces()
    {
        AmountOccupedPlaces = 0;
        AmountAvailablePlaces = 0;
        
        foreach (SitPlace place in sitPlaces)
        {
            if(!place.Available)
            {
                AmountOccupedPlaces = Mathf.Min(AmountOccupedPlaces+1, sitPlaces.Length);
                AmountAvailablePlaces = Mathf.Max(AmountAvailablePlaces-1, 0);
            }
            else
            {
                AmountAvailablePlaces = Mathf.Min(AmountAvailablePlaces+1, sitPlaces.Length);
                AmountOccupedPlaces = Mathf.Max(AmountOccupedPlaces-1, 0);
            }
        }
    }
}
