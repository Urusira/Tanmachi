using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.World;
using UnityEngine;


[DefaultExecutionOrder(0)]
public class ObjectPlacer : MonoBehaviour
{
    public event System.Action<ItemSO> OnPlaceableObjectHasPlaced;
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    
    [Header("Placement Parameters")]
    [SerializeField] private float fastObjectRotationMultiplier = 15f;
    [SerializeField] private float objectRotationMultiplier = 5f;
    
    [Header("Raycast Parameters")]
    [SerializeField] private float objectDistanceFromPlayer;
    [SerializeField] private float raycastStartVerticalOffset;
    [SerializeField] private float raycastDistance;
    
    [Header("Preview Material")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;
    
    private ItemSO _placeableObjectItemSO;
    private GameObject _placeableObjectPrefab;
    private GameObject _previewObjectPrefab;
    
    private GameObject _previewObject;
    
    private PreviewObjectValidChecker _previewObjectValidChecker;
    
    private Vector3 _currentPlacementPosition = Vector3.zero;
    private bool _inPlacementMode = false;
    private bool _validPreviewState = false;

    private float _rotateOffset = 0f;
    
    private void Update()
    {
        if (_inPlacementMode)
        {
            print("В режиме размещения");
            UpdateCurrentPlacementPosition();

            if (CanPlaceObject())
            {
                print("Можем разместить");
                SetValidPreviewState();
            }
            else
            {
                print("Не можем разместить");
                SetInvalidPreviewState();
            }
        }
    }

    private void SetValidPreviewState()
    {
        GuiManager.Instance.SetHint("Нажмите F чтобы разместить");
        previewMaterial.color = validColor;
        _validPreviewState = true;
    }
    
    private void SetInvalidPreviewState()
    {
        GuiManager.Instance.SetHint("Нельзя разместить");
        previewMaterial.color = invalidColor;
        _validPreviewState = false;
    }
    
    private bool CanPlaceObject()
    {
        print("Проверяем, можем ли разместить");
        if (_previewObject == null) return false;

        return _previewObjectValidChecker.IsValid;
    }
    
    private void UpdateCurrentPlacementPosition()
    {
        Vector3 cameraForward = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z);
        cameraForward.Normalize();
        
        Vector3 startPos = playerCamera.transform.position + (cameraForward * objectDistanceFromPlayer);
        startPos.y += raycastStartVerticalOffset;
        
        RaycastHit hitInfo;
        if (Physics.Raycast(startPos, Vector3.down, out hitInfo, raycastDistance, LayerManager.Instance.BuildLayers))
        {
            _currentPlacementPosition = hitInfo.point;
        }

        Quaternion rotation = Quaternion.Euler(0f, (playerCamera.transform.eulerAngles.y + _rotateOffset) % 360, 0f);
        _previewObject.transform.position = _currentPlacementPosition;
        _previewObject.transform.rotation = rotation;
    }

    public void PlaceObject()
    {
        if (_inPlacementMode && _validPreviewState)
        {
            Quaternion rotation = Quaternion.Euler(0, (playerCamera.transform.eulerAngles.y + _rotateOffset) % 360, 0);
            Instantiate(_placeableObjectPrefab, _currentPlacementPosition, rotation, transform);
            
            OnPlaceableObjectHasPlaced?.Invoke(_placeableObjectItemSO);
        }
    }

    public void RotateObject(float direction, bool fastRotate)
    {
        if (_inPlacementMode)
        {
            if (direction > 0)
            {
                _rotateOffset += 1 * (fastRotate == true ? fastObjectRotationMultiplier : objectRotationMultiplier);
            }
            else
            {
                _rotateOffset -= 1 * (fastRotate == true ? fastObjectRotationMultiplier : objectRotationMultiplier);
            }
        }
    }

    public void ObjectSet(GameObject objectPrefab, GameObject objectPreviewPrefab, ItemSO objectItemSO)
    {
        _placeableObjectPrefab = objectPrefab;
        _previewObjectPrefab = objectPreviewPrefab;
        _placeableObjectItemSO = objectItemSO;
    }

    public void EnterPlacementMode()
    {
        //PlayerInputControllersRegulator.Instance.EnterPlacementMode();
        Quaternion rotation = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0);
        _previewObject = Instantiate(_previewObjectPrefab, _currentPlacementPosition, rotation, transform);

        _previewObjectValidChecker = _previewObject.GetComponent<PreviewObjectValidChecker>();
        
        _inPlacementMode = true;
    }

    public void ExitPlacementMode()
    {
        //PlayerInputControllersRegulator.Instance.ExitPlacementMode();
        _placeableObjectPrefab = null;
        _previewObjectPrefab = null;
        _placeableObjectItemSO = null;
        
        _inPlacementMode = false;
        
        GuiManager.Instance.HideHint();
        
        Destroy(_previewObject);
    }
}
