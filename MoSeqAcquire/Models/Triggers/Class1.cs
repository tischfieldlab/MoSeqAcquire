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
        protected List<Type> triggers;
        protected Dictionary<Type, List<TriggerAction>> subscribers;

        public void Register<TTrigger>() where TTrigger : Trigger
        {
            this.triggers.Add(typeof(TTrigger));
        }
        public void Subscribe<TTrigger>(TriggerAction triggerAction) where TTrigger : Trigger
        {
            if (!this.subscribers.ContainsKey(typeof(TTrigger)))
            {
                this.subscribers[typeof(TTrigger)] = new List<TriggerAction>();
            }
            this.subscribers[typeof(TTrigger)].Add(triggerAction);
        }

        public void Trigger<TTrigger>(TTrigger trigger) where TTrigger : Trigger
        {
            this.subscribers[typeof(TTrigger)].ForEach(t => t.Action.Invoke(trigger));
        }
    }


    public abstract class Trigger
    {
        public string Name { get; }
        
    }

    public abstract class TriggerConfig : BaseConfiguration { }

    public class TriggerAction
    {
        public TriggerConfig Config { get; set; }
        public Action<Trigger> Action { get; }
    }


    public class RecordingStartedTrigger : Trigger { }
}
