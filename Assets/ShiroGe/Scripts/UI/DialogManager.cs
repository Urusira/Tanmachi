using ShiroGe.CharacterController;
using ShiroGe.Scripts.LLM.Data.Repository;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject responseFieldObj;
    [SerializeField] private GameObject thinksFieldObj;
    [SerializeField] private GameObject playerObj;
    
    public bool InDialog { get; private set; }

    private PlayerController _playerController;
    
    private TextMeshProUGUI responseField;
    private TMP_InputField thinksField;
    
    public string currTalkativeNpcId { get; private set; }

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
        currTalkativeNpcId = npcId;
        
        responseField.text = $"{npcName}\n\n" + string.Join("\n", NpcDialogRepository.Instance.GetNpcHistoryUI(currTalkativeNpcId));
        
        ShowDialogUI();
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

    private void ShowDialogUI()
    {
        InDialog = true;
        dialogCanvas.SetActive(true);
        GuiManager.Instance.UnlockMouse();
        GuiManager.Instance.HideGui();
        _playerController.LockControl(blockInventoryControl: true);
    }
    
    private void HideDialogUI()
    {
        InDialog = false;
        dialogCanvas.SetActive(false);
        GuiManager.Instance.LockMouse();
        GuiManager.Instance.ShowGui();
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