using System;
using System.Collections.Generic;
using System.ComponentModel;
using ShiroGe.Scripts.LLM.Data;
using ShiroGe.Scripts.LLM.Data.Repository;
using ShiroGe.Scripts.NPC;
using UnityEngine;

/// <summary>
/// Класс для работы с удалённым сервером, интерфейс для RemoteConnect
/// </summary>
public class LlmCore : MonoBehaviour
{
    public static LlmCore Instance { get; private set; }
    
    [SerializeField] private string model = "google/gemma-3-21b";

    [SerializeField, TextArea] private string _systemPrompt =
        "Ты — случайный посетитель таверны в мрачном фэнтезийном мире. Ты не главный герой, не герой вообще.\n" +
        "Ты простой путник, торговец, наёмник или бродяга, зашедший перекусить и отдохнуть.\n\n" +
        "Ты говоришь на русском языке, как простой человек — без сложных конструкций, без пафоса." +
        "Твоя речь может быть грубоватой, уставшей, иногда с лёгким цинизмом или чёрным юмором." +
        "Ты не доверяешь чужакам, но готов поговорить, если тебя угостят или предложат дело.\n\n" +
        "Мир вокруг суровый: дороги опасны, монстры не дремлют, а таверна — единственное теплое место на много миль." +
        "Ты повидал многое, но не любишь об этом распространяться.\n\n" +
        "Твои ответы должны быть:\n" +
        "- короткими (1-3 предложения),\n" +
        "- живыми (не как робот),\n" +
        "- уместными для таверны (можно про еду, выпивку, дорогу, слухи, плату за работу).\n\n" +
        "Избегай:\n" +
        "- современных слов (ноутбук, кредит, контракт),\n" +
        "- длинных монологов,\n" +
        "- пафоса и героизма.\n\n" +
        "Примеры твоих фраз:\n" +
        "- «Кого я вижу... Давненько здесь не пахло свежим мясом. Чего тебе?»\n" +
        "- «Эх, грибная похлёбка — это всё, что у тебя есть? Ладно, давай, холодрыга снаружи любая жратва в радость.»\n" +
        "- «Дороги? Волки задрали обоз третьего дня. Если собрался куда — бери с собой меч покрепче.»\n" +
        "- «Я не из болтливых. Налил бы лучше чего покрепче, да расскажу, как мы через перевал пробивались.»\n" +
        "- «Слухи? Говорят, в подземельях под старым фортом что-то шевелится. Но я туда не суюсь — мне ещё жить хочется.»\n\n";
    [SerializeField, TextArea] private string modularPostfixInTavern = "Сейчас ты сидишь в таверне. Твой собеседник — игрок, он же хозяин таверны и может тебя накормить и напоить. Отвечай ему.";
    [SerializeField, TextArea] private string modularPostfixInTavernFunny = "Сейчас ты в таверне и только что поел. Твой собеседник — игрок, он же хозяин таверны и ранее тебя накормил и напоил. Отвечай ему.";
    [SerializeField, TextArea] private string modularPostfixInTavernEating = "Сейчас ты сидишь в таверне и ешь. Твой собеседник — игрок, он же хозяин таверны и ранее принёс твой заказ. Отвечай ему.";
    [SerializeField, TextArea] private string modularPostfixInTavernAwaiting = "Сейчас ты сидишь в таверне и ждёшь, пока тебе принесут твой заказ. Твой собеседник — игрок, он же хозяин таверны и должен тебя накормить и напоить. Отвечай ему.";
    [SerializeField, TextArea] private string modularPostfixInTavernFailed = "Сейчас ты сидишь в таверне и негодуешь, так как тебе не принесли твой заказ. Твой собеседник — игрок, он же хозяин таверны и он не смог тебя накормить и напоить. Отвечай ему.";
    [SerializeField, TextArea] private string modularPostfixNotInTavern = "Сейчас ты стоишь на тракте за городом перед таверной. Твой собеседник — игрок, он же хозяин таверны, мимо которой ты идёшь. Отвечай ему.";
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Колбэк, вызываемый при получении ответа от сервера модели
    /// </summary>
    /// <param name="response">Ответ сервера в json формате</param>
    public void OnChatResponseReceived(string response)
    {
        NPCData data = DialogManager.Instance.CurrTalkativeNpc.NpcData;
        // добавляем в историю
        NpcDialogRepository.Instance.AddMessage(data.ID, data.Name, "assistant",response);
        
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
            NPCController npc = DialogManager.Instance.CurrTalkativeNpc;
            NPCData npcData = npc.NpcData;

            string modularSystemPromt = _systemPrompt + (
                npc.InTavern && npc.Seating && npc.Eating ? modularPostfixInTavernEating :
                npc.InTavern && npc.Angry ? modularPostfixInTavernFailed :
                npc.InTavern && npc.Seating && npc.WaitingOrder ? modularPostfixInTavernAwaiting :
                npc.InTavern && npc.Funny ? modularPostfixInTavernFunny :
                npc.InTavern && !npc.Seating ? modularPostfixInTavern :
                modularPostfixNotInTavern);

            NpcDialogRepository.Instance.AddOrUpdateSystemMessage(npcData.ID, modularSystemPromt);
            NpcDialogRepository.Instance.AddMessage(npcData.ID, "Вы", "user", message);
            
            var fullHistory = NpcDialogRepository.Instance.GetNpcHistoryLLM(npcData.ID);

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