using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    public class TriggerBus
    {
        protected Dictionary<Type, List<TriggerAction>> subscribers;

        public TriggerBus()
        {
            this.subscribers = new Dictionary<Type, List<TriggerAction>>();
        }
        
        public void Subscribe(Type Trigger, TriggerAction triggerAction)
        {
            if (!this.subscribers.ContainsKey(Trigger))
            {
                this.subscribers[Trigger] = new List<TriggerAction>();
            }
            this.subscribers[Trigger].Add(triggerAction);
        }
        public void Unsubscribe(Type Trigger, TriggerAction triggerAction)
        {
            this.subscribers[Trigger].Remove(triggerAction);
        }

        public void Trigger<TTrigger>(TTrigger trigger) where TTrigger : Trigger
        {
            this.subscribers[typeof(TTrigger)].ForEach(t => t.Action.Invoke(trigger));
        }
    }
}
