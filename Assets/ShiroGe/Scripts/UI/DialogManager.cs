using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.LLM.Data.Repository;
using ShiroGe.Scripts.Quests;
using ShiroGe.Scripts.UI;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private GameObject responseFieldObj;
    [SerializeField] private GameObject thinksFieldObj;
    [SerializeField] private GameObject playerObj;
    
    public bool InDialog { get; private set; }

    public bool onlineStrategy = true;

    private PlayerController _playerController;
    
    private TextMeshProUGUI responseField;
    private TMP_InputField thinksField;
    
    public string currTalkativeNpcId { get; private set; }

    private void Awake()
    {
        dialogueRunner.AddFunction("GiveQuest", (string questId) => {
            QuestOrderManager.Instance.CreateTestOrder(currTalkativeNpcId, questId);
            return 0;
        });
        
        dialogueRunner.AddFunction("HasQuest", (string questId) => {
            return QuestOrderManager.Instance.HasQuest(currTalkativeNpcId, questId); 
        });
        
        dialogueRunner.AddFunction("QuestStatusCheck", (string questId) => {
            return QuestOrderManager.Instance.QuestStatusCheck(currTalkativeNpcId, questId).ToString();
        });
        
        dialogueRunner.AddFunction("QuestCompleteConditionCheck", (string questId) => {
            return QuestOrderManager.Instance.QuestCompleteConditionCheck(currTalkativeNpcId, questId); 
        });
        
        dialogueRunner.AddFunction("CompleteQuest", (string questId) => { 
            QuestOrderManager.Instance.QuestComplete(currTalkativeNpcId, questId);
            return 0;
        });
        
        dialogueRunner.AddFunction("CancelQuest", (string questId) => { 
            QuestOrderManager.Instance.CancelQuest(currTalkativeNpcId, questId);
            return 0;
        });
    }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        //DontDestroyOnLoad(gameObject);
        
        responseField = responseFieldObj.GetComponent<TextMeshProUGUI>();
        thinksField = thinksFieldObj.GetComponent<TMP_InputField>();
        
        _playerController = playerObj.GetComponent<PlayerController>();
        
        HideDialogUI();
    }


    public void StartDialog(string npcName, string npcId)
    {
        if(onlineStrategy)
        {
            currTalkativeNpcId = npcId;

            responseField.text = $"{npcName}\n\n" +
                                 string.Join("\n", NpcDialogRepository.Instance.GetNpcHistoryUI(currTalkativeNpcId));

            ShowOnlineDialogUI();
        }
        else
        {
            currTalkativeNpcId = npcId;
            
            ShowOfflineDialogUI();
        }
    }

    public void CloseDialog()
    {
        responseField.text = "";
        thinksField.text = "";
        
        HideDialogUI();
    }

    public void Send()
    {
        string message = thinksField.text;
        if (string.IsNullOrEmpty(message)) return;
        
        responseField.text += $"Игрок: {message}\n\n";
        
        LlmCore.Instance.OnUserMessageSent(message);
    }

    public void Response(string response)
    {
        responseField.text += $"НПС: {response}\n\n";
    }

    private void ShowOnlineDialogUI()
    {
        PlayerDialogBlock();
        dialogCanvas.SetActive(true);
    }
    
    private void ShowOfflineDialogUI()
    {
        PlayerDialogBlock();
        YarnTask task = dialogueRunner.StartDialogue("StandartNPCYarnScript");
    }
    
    private void HideDialogUI()
    {
        PlayerDialogUnblock();
        dialogCanvas.SetActive(false);
    }
    
    
    private void PlayerDialogBlock()
    {
        InDialog = true;
        GuiManager.Instance.UnlockMouse();
        GuiManager.Instance.HideGui();
        InventoryUiManager.Instance.HotbarHide();
        _playerController.LockControl(blockInventoryControl: true);
    }
    
    private void PlayerDialogUnblock()
    {
        InDialog = false;
        GuiManager.Instance.LockMouse();
        GuiManager.Instance.ShowGui();
        InventoryUiManager.Instance.HotbarShow();
        _playerController.UnlockControl();
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}