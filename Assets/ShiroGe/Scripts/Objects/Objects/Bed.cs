using System;
using System.Collections;
using DG.Tweening;
using ShiroGe.CharacterController;
using ShiroGe.Scripts;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.UI;
using ShiroGe.Scripts.World;
using UnityEngine;
using UnityEngine.Serialization;

public class Bed : Interactable
{
    public float toBedTransitionTime = 3;
    public float toBedAndSleepAwait = 2;
    public float sleepTransitionTime = 3;
    
    public bool isOccupped = false;
    
    [SerializeField] public GameObject sleepingHeadPosition;
    [SerializeField] public Camera overnightCamera;
    [SerializeField] public CanvasGroup sleepingUICanvas;

    private GameObject _sleeper;
    private GameObject _mainCamera;
    private Vector3 _preSleepPosition;
    private Vector3 _preSleepRotation;

    private void Start()
    {
        _mainCamera = Camera.main?.gameObject;
    }

    protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
    {
        if (isOccupped) return PlayerActionsState.Default;
        
        isOccupped = true;
        
        _sleeper = player;
        
        sleepingUICanvas.gameObject.SetActive(true);
        sleepingUICanvas.alpha = 0;
        try
        {
            StartCoroutine(SleepTransitionCoroutine());
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        
        return PlayerActionsState.Default;
    }

    private IEnumerator SleepTransitionCoroutine()
    {
        GuiManager.Instance.HideGui();
        InventoryUiManager.Instance.HotbarHide();
        
        if(_sleeper != null)
        {
            _sleeper.GetComponent<PlayerController>().LockControl(true);
            _sleeper.SetActive(false);
            CameraFollowTarget.Instance.enabled = false;
            _mainCamera.SetActive(true);
            
            _preSleepPosition = _mainCamera.transform.position;
            _preSleepRotation = _mainCamera.transform.rotation.eulerAngles;
        }
        
        _mainCamera?.transform.DOMove(sleepingHeadPosition.transform.position, toBedTransitionTime).SetEase(Ease.InOutBounce);
        _mainCamera?.transform.DORotate(sleepingHeadPosition.transform.eulerAngles, toBedTransitionTime);
        
        yield return new WaitForSeconds(toBedTransitionTime+toBedAndSleepAwait);

        sleepingUICanvas.DOFade(1f, sleepTransitionTime).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(sleepTransitionTime);
        
        _mainCamera?.SetActive(false);
        
        TimeManager.Instance.SkipDayPhase();
        sleepingUICanvas.gameObject.SetActive(false);
        overnightCamera?.gameObject.SetActive(true);
        
        TimeManager.Instance.OnDayPhaseChanged += SleepingAwaiter;
    }

    private void SleepingAwaiter(DayPhase _)
    {
        sleepingUICanvas.gameObject.SetActive(true);
        StartCoroutine(AwakeTransitionCoroutine());
    }

    private IEnumerator AwakeTransitionCoroutine()
    {
        TimeManager.Instance.OnDayPhaseChanged -= SleepingAwaiter;
        
        overnightCamera?.gameObject.SetActive(false);
        _mainCamera?.SetActive(true);
        
        sleepingUICanvas.enabled = true;
        
        sleepingUICanvas.DOFade(0f, sleepTransitionTime).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(sleepTransitionTime);
        
        sleepingUICanvas.gameObject.SetActive(false);
        
        _mainCamera?.transform.DOMove(_preSleepPosition, toBedTransitionTime).SetEase(Ease.InBounce);
        _mainCamera?.transform.DORotate(_preSleepRotation, toBedTransitionTime);
        yield return new WaitForSeconds(toBedTransitionTime);
        
        _sleeper?.SetActive(true);
        _sleeper?.GetComponent<PlayerController>()?.UnlockControl();
        
        GuiManager.Instance.ShowGui();
        InventoryUiManager.Instance.HotbarShow();
        
        CameraFollowTarget.Instance.enabled = true;
        
        isOccupped = false;
    }

    protected override NPCActionsState NpcOverridableInteract(GameObject npc)
    {
        throw new System.NotImplementedException();
    }

    protected override void Initiate()
    {
        return;
    }

    public override string ShowHint()
    {
        base.ShowHint();
        return "Нажмите F чтобы спать";
    }
}
