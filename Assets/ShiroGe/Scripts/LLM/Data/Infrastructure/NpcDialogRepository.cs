using System.Collections.Generic;
using UnityEngine;

namespace ShiroGe.Scripts.LLM.Data.Repository
{
    
    [System.Serializable]
    public class NpcDialogData 
    { 
        public string npcId;
        public List<string> history = new();  // просто строки для UI
        public List<Message> llmHistory = new();  // для LlmCore
    }

    public class NpcDialogRepository : MonoBehaviour
    {
        public static NpcDialogRepository Instance { get; private set; }
    
        private Dictionary<string, NpcDialogData> npcHistories = new();
    
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    
        public NpcDialogData GetOrCreate(string npcId)
        {
            if (!npcHistories.ContainsKey(npcId))
            {
                npcHistories[npcId] = new NpcDialogData { npcId = npcId };
            }
            return npcHistories[npcId];
        }
    
        // Получить историю для UI отображения
        public List<string> GetNpcHistoryUI(string npcId)
        {
            var data = GetOrCreate(npcId);
            return data.history;
        }
    
        // Получить историю для LLM
        public List<Message> GetNpcHistoryLLM(string npcId)
        {
            return GetOrCreate(npcId).llmHistory;
        }
    
        // Добавить сообщение
        public void AddMessage(string npcId, string uiRole, string llmRole, string content)
        {
            var data = GetOrCreate(npcId);
            data.history.Add($"{uiRole}: {content}");
            data.llmHistory.Add(new Message { role = llmRole, content = content });
        }

        public void AddOrUpdateSystemMessage(string npcId, string systemMessage)
        {
            var data = GetOrCreate(npcId);

            // Добавление системного сообщения в историю
            if ((data.llmHistory.Count == 0 || data.llmHistory[0].role != "system") && !string.IsNullOrEmpty(systemMessage))
            {
                data.llmHistory.Insert(0, new Message { role = "system", content = systemMessage });
            }
        }
    }

}