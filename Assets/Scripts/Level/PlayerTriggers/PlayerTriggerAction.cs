using System;

namespace Game.Level.Triggers
{
    [Serializable]
    public abstract class PlayerTriggerAction
    {
        public virtual void OnTriggerEnter() { }

        public virtual void OnTriggerExit() { }
    }
}