namespace ShiroGe.Scripts.NPC
{
    public class NPCData
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public readonly string Personality;
        public readonly int Age;
        public readonly bool Introvert;

        public NPCData(string id, string name, string personality, int age, bool introvert)
        {
            ID = id;
            Name = name;
            Personality = personality;
            Age = age;
            Introvert = introvert;
        }
        
        public void SetId(string id)
        {
            ID = id;
        }
        
        public void SetNewName(string newName)
        {
            Name = newName;
        }
    }
}