using System.Collections.Generic;
using ShiroGe.Scripts.Quests;

namespace ShiroGe.Scripts
{
    public abstract class Transaction
    {
        protected object From;
        protected object To;
        protected int Cash;
        protected HashSet<ItemWithAmount> Items;
    
        public abstract bool Validate();
        public abstract void Commit();
        public abstract void Rollback();
    }
    
}