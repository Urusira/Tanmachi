using System.ComponentModel;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollowTarget : MonoBehaviour
{
    public static CameraFollowTarget Instance { get; private set; }
    
    [SerializeField] private Transform targetAnchor;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

    private Camera _camera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _camera = Camera.main;
        
        if (_camera == null)
        {
            throw new WarningException("Scene haven't main camera");
        }
    }

    private void LateUpdate()
    {
        if (targetAnchor == null) return;

        Vector3 worldOffset = targetAnchor.TransformDirection(offset);
        Vector3 targetPosition = targetAnchor.position + worldOffset;

        _camera.transform.position = targetPosition;
    }
}