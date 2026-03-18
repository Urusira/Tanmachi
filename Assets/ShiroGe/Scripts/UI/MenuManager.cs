using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    //[SerializeField] private GameObject cinemachineCameraObj;

    //private CinemachinePanTilt cameraController;

    public static MenuManager Instance { get; private set; }

    public void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //cameraController = cinemachineCameraObj.GetComponent<CinemachinePanTilt>();
        hideMenu();
    }

    public void showMenu()
    {
        menuCanvas.SetActive(true);
        GuiManager.Instance.HideGui();
        //.Instance.ChangeInputType(InputManager.InputType.UI);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //cameraController.PanAxis.Range = new Vector2(cameraController.PanAxis.Value, cameraController.PanAxis.Value);
        //cameraController.TiltAxis.Range = new Vector2(cameraController.TiltAxis.Value, cameraController.TiltAxis.Value);

    }

    public void hideMenu()
    {
        menuCanvas.SetActive(false);
        GuiManager.Instance.ShowGui();
        //InputManager.Instance.ChangeInputType(InputManager.InputType.Player);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        //cameraController.PanAxis.Range = new Vector2(-180, 180);
        //cameraController.TiltAxis.Range = new Vector2(-70, 70);
    }

    public void exit()
    {
        //TODO: ����� �� ����
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}