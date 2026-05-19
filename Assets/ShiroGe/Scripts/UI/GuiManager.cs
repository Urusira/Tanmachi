using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public static GuiManager Instance { get; private set; }

    [SerializeField] private GameObject guiCanvas;
    [SerializeField] private GameObject pointerObj;
    [SerializeField] private GameObject hintTextObj;

    [Header("Settings")]
    [SerializeField] private Vector2 stdPointerSize;
    [SerializeField] private Vector2 highlightPointerSize;
    [SerializeField] private Color stdPointerColor;
    [SerializeField] private Color highlightPointerColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField] private String standartHint = "F для телепортации";

    private Image _pointer;
    private TextMeshProUGUI _hintTextMesh;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        stdPointerColor = pointerObj.GetComponent<Image>().color;
        _hintTextMesh = hintTextObj.GetComponent<TextMeshProUGUI>();

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        _pointer = pointerObj.GetComponent<Image>();
        ShowGui();
    }

    public void HighlightPointer(String hintText = "")
    {
        _pointer.color = highlightPointerColor;
        _pointer.rectTransform.sizeDelta = highlightPointerSize;
        _hintTextMesh.text = hintText == "" ? standartHint : hintText;
    }

    public void ResetPointer()
    {
        _pointer.color = stdPointerColor;
        _pointer.rectTransform.sizeDelta = stdPointerSize;
        _hintTextMesh.text = standartHint;
    }

    public void HideGui()
    {
        guiCanvas.SetActive(false);
    }
    public void ShowGui()
    {
        guiCanvas.SetActive(true);
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState     = CursorLockMode.None;
        Cursor.visible = true;
    }
}