using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public static GuiManager Instance { get; private set; }

    [SerializeField] private GameObject guiCanvas;
    [SerializeField] private GameObject pointerObj;

    [Header("Settings")]
    [SerializeField] private Vector2 stdPointerSize;
    [SerializeField] private Vector2 highlightPointerSize;
    [SerializeField] private Color stdPointerColor;
    [SerializeField] private Color highlightPointerColor = new Color(1f, 1f, 1f, 0.8f);

    private Image _pointer;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        stdPointerColor = pointerObj.GetComponent<Image>().color;
        stdPointerSize = pointerObj.GetComponent<RectTransform>().sizeDelta;
        highlightPointerSize = stdPointerSize * 1.2f;

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _pointer = pointerObj.GetComponent<Image>();
        ShowGui();
    }

    public void HighlightPointer()
    {
        _pointer.color = highlightPointerColor;
        _pointer.rectTransform.sizeDelta = highlightPointerSize;
    }

    public void ResetPointer()
    {
        _pointer.color = stdPointerColor;
        _pointer.rectTransform.sizeDelta = stdPointerSize;
    }

    public void HideGui()
    {
        guiCanvas.SetActive(false);
    }
    public void ShowGui()
    {
        guiCanvas.SetActive(true);
    }
}