using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject cinemachineCameraObj;

    private TextMeshProUGUI responseField;
    private TMP_InputField thinksField;
    //private CinemachinePanTilt cameraController;
    
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
        //cameraController = cinemachineCameraObj.GetComponent<CinemachinePanTilt>();
        HideDialogUI();
    }


    public void StartDialog(string npcName, string npcId)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        currTalkativeNpcId = npcId;
        
        var npcHistory = NpcDialogRepository.Instance.GetNpcHistoryUI(currTalkativeNpcId);
        responseField.text = $"{npcName}\n\n" + string.Join("\n", npcHistory);
        ShowDialogUI();
        /*cameraController.PanAxis.Wrap = false;
        cameraController.PanAxis.Range = new Vector2(cameraController.PanAxis.Value - 10, cameraController.PanAxis.Value + 10);
        cameraController.TiltAxis.Range = new Vector2(cameraController.TiltAxis.Value - 5, cameraController.TiltAxis.Value + 5);*/
    }

    public void CloseDialog()
    {
        responseField.text = "";
        thinksField.text = "";
        HideDialogUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        /*cameraController.PanAxis.Wrap = true;
        cameraController.PanAxis.Range = new Vector2(-180, 180);
        cameraController.TiltAxis.Range = new Vector2(-70, 70);*/
    }

    public void Send()
    {
        string message = thinksField.text;
        if (string.IsNullOrEmpty(message)) return;
        
        responseField.text += $"\tИгрок: {message}\n\n";
        
        // Передаём в LlmCore ЭТУ историю НПС
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