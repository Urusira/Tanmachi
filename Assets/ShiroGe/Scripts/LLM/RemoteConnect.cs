using System;
using System.Text;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShiroGe.Scripts.LLM.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class RemoteConnect : MonoBehaviour
{
    public static RemoteConnect Instance { get; private set; }

    [SerializeField] private string serverAddress = "https://tentatively-tenacious-wolffish.cloudpub.ru";
    [SerializeField] private bool debug = false;


    public void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void sendToAi(AiRequest userRequest)
    {
        if (debug)
        {
            LlmCore.Instance.OnChatResponseReceived("Дебажный ответ");
            return;
        }

        StartCoroutine(sendToAiCoroutine(userRequest));
    }
    
    public IEnumerator sendToAiCoroutine(AiRequest userRequest)
    {

        string bundle = JsonUtility.ToJson(userRequest);

        using (UnityWebRequest request = new UnityWebRequest(serverAddress, "POST"))
        {
            byte[] raw = Encoding.UTF8.GetBytes(bundle);
            request.uploadHandler = new UploadHandlerRaw(raw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            Debug.Log("Заебись6");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка связи:\n" + request.error);
            }
            else
            {
                string aiResponse = ParseAiResponse(request.downloadHandler.text);
                LlmCore.Instance.OnChatResponseReceived(aiResponse);
            }
        }
    }
    private string ParseAiResponse(string jsonResponse)
    {
        try
        {
            AiResponse resp = JsonUtility.FromJson<AiResponse>(jsonResponse);
            if (resp.choices == null || resp.choices.Length == 0)
            {
                Debug.LogError("Empty choices in response");
                return "Ошибка получения ответа";
            }

            string content = resp.choices[0].message.content;

            return content;
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка парсинга: {e.Message}\n{jsonResponse}");
            return "Ошибка получения ответа";
        }
    }
}