using System.Collections.Generic;
using ShiroGe.Scripts.Utils;
using ShiroGe.Scripts.World;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewObjectValidChecker : MonoBehaviour
{
    [SerializeField] private List<GameObject> collidingObjects =  new List<GameObject>();
    
    public bool IsValid { get; private set; } = true;

    private void OnTriggerEnter(Collider other)
    {
        if (LayerManager.Instance.NonBuildLayers.Contains(other.gameObject.layer))
        {
            collidingObjects.Add(other.gameObject);
            IsValid = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerManager.Instance.NonBuildLayers.Contains(other.gameObject.layer))
        {
            collidingObjects.Remove(other.gameObject);
            IsValid = collidingObjects.Count <= 0;
        }
    }
}
