using System;
using ShiroGe.Scripts;
using ShiroGe.Scripts.World;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    [RequireComponent(typeof(PlayerState))]
    [RequireComponent(typeof(PlayerActionsController))]
    public class PlayerInteractionController : MonoBehaviour
    {
        [Header("Interaction parameters")]
        [SerializeField] public float interactionDistance = 3f;
        
        private PlayerActionsController _playerActionsContoller;
        private PlayerState _playerState;
        
        private Camera _playerCamera;
        
        private bool _canInteract = true;
        private bool _interactLastFrame = false;
        
        private GameObject _target;
        private Interactable _targetInteractComponent;
        
        public Vector3 GlobalRaycastHit { get; private set; }
        public Vector3 RawView { get; private set; }
        
        private void Awake()
        {
            _playerCamera = Camera.main;
            _playerState = GetComponent<PlayerState>();
            _playerActionsContoller = GetComponent<PlayerActionsController>();
        }

        private void Update()
        {
            PointerScan();
        }
        
        private void PointerScan()
        {
            if(_interactLastFrame && !_playerActionsContoller.InteractInput)
            {
                _interactLastFrame = false;
            }
            
            RawView = _playerCamera.transform.position + _playerCamera.transform.forward * interactionDistance;
            
            RaycastHit hit;
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward.normalized, out hit,
                    interactionDistance, LayerManager.Instance.InteractiveLayerMask))
            {
                if (_target != null && !_target.Equals(hit.collider.gameObject))
                {
                    if (_targetInteractComponent != null)
                    {
                        _targetInteractComponent.HideHint();
                    }
                }

                _target = hit.collider.gameObject;
                _targetInteractComponent = _target.GetComponent<Interactable>();
                
                if(_targetInteractComponent != null)
                {
                    String hintText = _targetInteractComponent.ShowHint();
                    GuiManager.Instance.HighlightPointer(hintText);
                }
            }
            else if (_target != null)
            {
                if (_targetInteractComponent != null)
                {
                    _targetInteractComponent.HideHint();
                }
                _playerActionsContoller.SetInteractPressedFalse(); //TODO: Заглушка-фикс, надо убрать
                _target = null;
                GuiManager.Instance.ResetPointer();
            }
            else
            {
                GuiManager.Instance.ResetPointer();
            }
            
            if(!_interactLastFrame && _playerActionsContoller.InteractInput) TryInteract();
            
            RaycastHit hit2;
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward.normalized, out hit2,
                    interactionDistance, LayerManager.Instance.CollisiveLayers))
            {
                GlobalRaycastHit = hit2.point;
            }
            else
            {
                GlobalRaycastHit = Vector3.zero;
            }
        }

        private void TryInteract()
        {
            if (_target != null && _canInteract)
            {
                _interactLastFrame = true;
                PlayerActionsState typeAction = _target.GetComponent<Interactable>().PlayerInteract(gameObject);
                _playerState.SetPlayerActionsState(typeAction);
                switch (typeAction)
                {
                    case PlayerActionsState.Default:
                    {
                        _playerActionsContoller.SetInteractPressedFalse();
                        break;
                    }
                }
            }
        }

        public void InteractionsBlock()
        {
            Interactable targetInteractable;
            if (_target != null)
            {
                _target.TryGetComponent(out targetInteractable);
                targetInteractable.HideHint();
                _target = null;
            }

            _canInteract = false;
            _interactLastFrame = false;
        }

        public void InteractionsUnblock()
        {
            _canInteract = true;
        }
    }
}