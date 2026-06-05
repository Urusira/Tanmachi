namespace ShiroGe.Scripts.NPC
{
    public class NPCData
    {
        public string ID { get; private set; }
        public readonly string Name;
        public readonly string Personality;
        public readonly int Age;
        public readonly bool Loner;

        public NPCData(string id, string name, string personality, int age, bool loner)
        {
            ID = id;
            Name = name;
            Personality = personality;
            Age = age;
            Loner = loner;
        }
        
        public void SetId(string id)
        {
            ID = id;
        }
    }
}