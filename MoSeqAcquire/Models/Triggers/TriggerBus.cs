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
        protected Dictionary<Type, SortedSet<TriggerAction>> subscribers;

        public TriggerBus()
        {
            this.subscribers = new Dictionary<Type, SortedSet<TriggerAction>>();
        }
        

        public void Subscribe(Type Trigger, TriggerAction triggerAction)
        {
            if (!this.subscribers.ContainsKey(Trigger))
            {
                this.subscribers[Trigger] = new SortedSet<TriggerAction>(new PriorityComparer());
            }
            this.subscribers[Trigger].Add(triggerAction);
        }
        public void Unsubscribe(Type Trigger, TriggerAction triggerAction)
        {
            this.subscribers[Trigger].Remove(triggerAction);
        }

        public void Trigger<TTrigger>(TTrigger trigger) where TTrigger : TriggerEvent
        {
            if (this.subscribers.ContainsKey(typeof(TTrigger)))
            {
                var tasks = this.subscribers[typeof(TTrigger)]
                                .Select(t => Task.Run(() => {
                                    t.Execute(trigger);
                                })).ToArray();
                Task.WaitAll(tasks); // wait for all triggers to complete
            }
        }
    }

    public class PriorityComparer : IComparer<TriggerAction>
    {
        public int Compare(TriggerAction x, TriggerAction y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}
