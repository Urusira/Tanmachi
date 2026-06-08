namespace ShiroGe.Scripts.Tavern
{
    public static class TavernNPCSequence
    {
        private static int _npcNumberSequence = -1;
        
        public static int Get => ++_npcNumberSequence;
    }
}