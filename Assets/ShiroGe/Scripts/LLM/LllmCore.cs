using System;
using System.Collections.Generic;
using System.ComponentModel;
using ShiroGe.Scripts.LLM.Data;
using ShiroGe.Scripts.LLM.Data.Repository;
using UnityEngine;

/// <summary>
/// Класс для работы с удалённым сервером, интерфейс для RemoteConnect
/// </summary>
public class LlmCore : MonoBehaviour
{
    public static LlmCore Instance { get; private set; }
    
    [SerializeField] private string model = "google/gemma-3-12b";
    [SerializeField, TextArea] private string systemPrompt = "Ты локально запущен на моём сервере, я разговариваю с тобой через НПС в моей игре. " +
                                                             "Игра про управление таверной в фэнтезийном сеттинге. Мы не ролплеим, я проверяю " +
                                                             "работоспособность системы. Отвечай только на русском.";
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Колбэк, вызываемый при получении ответа от сервера модели
    /// </summary>
    /// <param name="response">Ответ сервера в json формате</param>
    public void OnChatResponseReceived(string response)
    {
        // добавляем в историю
        NpcDialogRepository.Instance.AddMessage(DialogManager.Instance.currTalkativeNpcId, "НПС", "assistant",response);
        
        DialogManager.Instance.Response(response);
    }

    /// <summary>
    /// Отправка пользователем сообщения на сервер модели
    /// </summary>
    /// <param name="message">Сообщение пользователя</param>
    /// <exception cref="WarningException">Предупреждение о пустом сообщении</exception>
    public void OnUserMessageSent(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new WarningException("Empty message");
            }
            
            NpcDialogRepository.Instance.AddOrUpdateSystemMessage(DialogManager.Instance.currTalkativeNpcId, systemPrompt);
            NpcDialogRepository.Instance.AddMessage(DialogManager.Instance.currTalkativeNpcId, "Игрок", "user", message);
            
            var fullHistory = NpcDialogRepository.Instance.GetNpcHistoryLLM(DialogManager.Instance.currTalkativeNpcId);

            RemoteConnect.Instance.sendToAi(new AiRequest
            {
                model = model,
                messages = fullHistory.ToArray(),
                stream = false
            });
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }
    }
}