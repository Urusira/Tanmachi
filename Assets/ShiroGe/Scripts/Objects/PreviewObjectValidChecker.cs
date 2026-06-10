using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewObjectValidChecker : MonoBehaviour
{
    [SerializeField] private LayerMask invalidLayers;
    [SerializeField] private List<GameObject> collidingObjects =  new List<GameObject>();
    
    public bool IsValid { get; private set; } = true;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & invalidLayers) != 0)
        {
            collidingObjects.Add(other.gameObject);
            IsValid = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & invalidLayers) != 0)
        {
            collidingObjects.Remove(other.gameObject);
            IsValid = collidingObjects.Count <= 0;
        }
    }
}
