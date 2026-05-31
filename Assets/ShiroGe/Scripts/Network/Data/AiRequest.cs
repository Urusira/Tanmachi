namespace ShiroGe.Scripts.LLM.Data
{
    [System.Serializable]
    public class AiRequest
    {
        public string model;
        public Message[] messages;
        public bool stream = false;
    }
}