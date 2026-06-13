using System;
using UnityEngine;

namespace ShiroGe.Scripts.NPC
{
    [Serializable]

    public class NPCData
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        public string Personality;
        public int Age;
        public bool Introvert;

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