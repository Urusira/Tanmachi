using System;
using System.Collections.Generic;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.LLM.Data.Repository;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject responseFieldObj;
    [SerializeField] private GameObject thinksFieldObj;
    [SerializeField] private GameObject playerObj;
    //[SerializeField] private GameObject cinemachineCameraObj;

    private TextMeshProUGUI responseField;
    private TMP_InputField thinksField;
    private PlayerState _playerState;
    
    public string currTalkativeNpcId { get; private set; }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        responseField = responseFieldObj.GetComponent<TextMeshProUGUI>();
        thinksField = thinksFieldObj.GetComponent<TMP_InputField>();
        _playerState = playerObj.GetComponent<PlayerState>();
        
        HideDialogUI();
    }


    public void StartDialog(string npcName, string npcId)
    {
        currTalkativeNpcId = npcId;
        
        responseField.text = $"{npcName}\n\n" + string.Join("\n", NpcDialogRepository.Instance.GetNpcHistoryUI(currTalkativeNpcId));
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        

        _playerState.InDialogChange();
        
        ShowDialogUI();
    }

    public void CloseDialog()
    {
        responseField.text = "";
        thinksField.text = "";
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _playerState.InDialogChange();
        
        HideDialogUI();
    }

    public void Send()
    {
        string message = thinksField.text;
        if (string.IsNullOrEmpty(message)) return;
        
        responseField.text += $"\tИгрок: {message}\n\n";
        
        LlmCore.Instance.OnUserMessageSent(message);
    }

    public void Response(string response)
    {
        responseField.text += $"\tНПС: {response}\n\n";
    }


    private void ShowDialogUI()
    {
        dialogCanvas.SetActive(true);
        GuiManager.Instance.HideGui();
    }
    private void HideDialogUI()
    {
        dialogCanvas.SetActive(false);
        GuiManager.Instance.ShowGui();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}