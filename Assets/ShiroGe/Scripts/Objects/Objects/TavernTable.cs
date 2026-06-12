using System;
using System.Collections.Generic;
using ShiroGe.Scripts;
using ShiroGe.Scripts.Items;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Tavern;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class TavernTable : MonoBehaviour
{
    public event System.Action<TavernTable> OnPlacesAvailabilityChanged;
    public event System.Action<TavernTable> OnPlacesReleased;
    public event System.Action<TavernTable> OnTableReleased;
    
    [SerializeField] private SitPlace[] sitPlaces;
    
    [field: SerializeField] public int AmountOccupedPlaces { get; private set; }  = 0;
    [field: SerializeField] public int AmountAvailablePlaces { get; private set; }  = 0;
    
    public bool TableFullReserved { get; private set; } = false;

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
        Interactable interactComponent;
        TryGetComponent(out interactComponent);

        if (interactComponent != null)
        {
            interactComponent.BlockPlayerInteractable();
        }

        RecountAvailablePlaces();
    }
    
    private void OnPlaceReservedHandler(SitPlace place, bool reserveFullTable)
    {
        TableFullReserved = reserveFullTable;
        
        /*if(forLoner)
        {
            foreach (SitPlace lonerOccupationPlace in sitPlaces)
            {
                lonerOccupationPlace.TryReservePlace(false);
            }
        }*/
        
        RecountAvailablePlaces();
        OnPlacesAvailabilityChanged?.Invoke(this);
    }
    
    private void OnPlaceVacatedHandler(SitPlace place, bool reserveFullTable)
    {
        if(reserveFullTable)
        {
            /*foreach (SitPlace lonerOccupationPlace in sitPlaces)
            {
                lonerOccupationPlace.UnreservePlace();
            }*/
            
            TableFullReserved = false;
        }
        
        RecountAvailablePlaces();
        OnPlacesReleased?.Invoke(this);
        if (AmountOccupedPlaces <= 0)
        {
            TableFullReserved = false;
            
            Interactable interactComponent;
            TryGetComponent(out interactComponent);

            if (interactComponent != null)
            {
                interactComponent.UnblockPlayerInteractable();
            }
            
            OnTableReleased?.Invoke(this);
        }
    }

    public List<SitPlace> GetAvailablePlaces(int reqAmo, bool ignoreFullReserved = false)
    {
        if (AmountAvailablePlaces < reqAmo || (!ignoreFullReserved && TableFullReserved)) return null;
        
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

    public SitPlace GetAvailablePlace(bool ignoreFullReserved = false)
    {
        if (AmountAvailablePlaces <= 0 || (!ignoreFullReserved && TableFullReserved)) return null;
        
        foreach (SitPlace place in sitPlaces)
        {
            if (place.Available)
            {
                return place;
            }
        }
        
        return null;
    }
    
    public SitPlace GetAvailableAndReservePlace(bool reserveFullTable, bool ignoreFullReserved = false)
    {
        if (AmountAvailablePlaces <= 0 || (!ignoreFullReserved && TableFullReserved)) return null;
        
        foreach (SitPlace place in sitPlaces)
        {
            if (place.Available)
            {
                bool succesful = place.TryReservePlace(reserveFullTable);
                
                if (!succesful) continue;
                
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
            if(place.Available)
            {
                AmountAvailablePlaces++;
            }
            else
            {
                AmountOccupedPlaces++;
            }
        }
    }

    private void OnDestroy()
    {
        TavernTablesManager.Instance.UnregisterTable(this);
    }
}
