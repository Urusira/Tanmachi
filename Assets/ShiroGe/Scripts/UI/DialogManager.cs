using System;
using ShiroGe.CharacterController;
using ShiroGe.Scripts.LLM.Data.Repository;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Quests;
using ShiroGe.Scripts.UI;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject responseFieldObj;
    [SerializeField] private GameObject thinksFieldObj;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject OfflineDialogUi;
    [SerializeField] private GameObject OnlineDialogUi;
    
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private LineAdvancer lineAdvancer;
    
    public bool InDialog { get; private set; }

    public bool onlineStrategy = true;

    private PlayerController _playerController;
    
    private TextMeshProUGUI responseField;
    private TMP_InputField thinksField;
    
    public NPCController CurrTalkativeNpc { get; private set; }

    private void Awake()
    {
        dialogueRunner.AddFunction("GetName", () => {
            return CurrTalkativeNpc.NpcData.Name;
        });
        
        dialogueRunner.AddFunction("GetLeaderName", () => {
            return CurrTalkativeNpc.GetGroupLeaderName();
        });
        
        dialogueRunner.AddFunction("GetQuest", (string questId) => {
            return CurrTalkativeNpc.GenerateQuest(questId);
        });
        
        dialogueRunner.AddFunction("StartQuest", (string questId) => {
            QuestOrderManager.Instance.StartQuest(CurrTalkativeNpc, questId);
            return 0;
        });
        
        dialogueRunner.AddFunction("NotAccepted", (string questId) => {
            CurrTalkativeNpc.QuestNotAccepted(questId);
            return 0;
        });
        
        dialogueRunner.AddFunction("HasQuest", (string questId) => {
            return QuestOrderManager.Instance.HasQuest(CurrTalkativeNpc, questId); 
        });
        
        dialogueRunner.AddFunction("QuestStatusCheck", (string questId) => {
            return QuestOrderManager.Instance.QuestStatusCheck(CurrTalkativeNpc, questId).ToString();
        });
        
        dialogueRunner.AddFunction("QuestCompleteConditionCheck", (string questId) => {
            return QuestOrderManager.Instance.QuestCompleteConditionCheck(CurrTalkativeNpc, questId); 
        });
        
        dialogueRunner.AddFunction("CompleteQuest", (string questId) => { 
            QuestOrderManager.Instance.QuestComplete(CurrTalkativeNpc, questId);
            return 0;
        });
        
        dialogueRunner.AddFunction("CancelQuest", (string questId) => { 
            QuestOrderManager.Instance.CancelQuest(CurrTalkativeNpc, questId);
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
        
        dialogCanvas.SetActive(false);
        OfflineDialogUi.SetActive(false);
        OnlineDialogUi.SetActive(false);
    }


    public void StartDialog(NPCController npc)
    {
        if (npc == null)
        {
            Debug.LogError("Npc is null, dialog cannot be started");
            return;
        }
        
        CurrTalkativeNpc = npc;
        
        CurrTalkativeNpc.LockMovement(PlayerInstance.Instance.GetPlayerRawStartPoint());
        PlayerDialogBlock();
        
        dialogCanvas.SetActive(true);
        
        if(onlineStrategy)
        {
            responseField.text = $"{npc.NpcData.Name}\n\n" +
                                 string.Join("\n", NpcDialogRepository.Instance.GetNpcHistoryUI(npc.NpcData.ID));

            ShowOnlineDialogUI();
        }
        else
        {
            ShowOfflineDialogUI();
        }
    }

    public void ChangeDialogMode()
    {
        CloseDialog();
        onlineStrategy = !onlineStrategy;
        StartDialog(CurrTalkativeNpc);
    }

    public void CloseDialog()
    {
        CurrTalkativeNpc.UnlockMovement();
        
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
        OnlineDialogUi.SetActive(true);
    }
    
    private void ShowOfflineDialogUI()
    {
        OfflineDialogUi.SetActive(true);
        YarnTask task = dialogueRunner.StartDialogue(CurrTalkativeNpc.GetActualDialog());
    }
    
    private void HideDialogUI()
    {
        PlayerDialogUnblock();
        
        if (onlineStrategy)
        {
            OnlineDialogUi.SetActive(false);
        }
        else
        {
            if (dialogueRunner.IsDialogueRunning)
            {
                try
                {
                    lineAdvancer?.RequestDialogueCancellation();
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError(e);
                    dialogueRunner.enabled = false;
                    dialogueRunner.enabled = true;
                }
            }
            OfflineDialogUi.SetActive(false);
        }
        
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