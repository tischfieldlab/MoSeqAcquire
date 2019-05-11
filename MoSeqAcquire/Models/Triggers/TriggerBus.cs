using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Views.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    [Serializable]
    public enum TriggerExecutionMode
    {
        Synchronous,
        Parallel
    }
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
            if (this.subscribers.ContainsKey(typeof(TTrigger)))
            {
                if (Properties.Settings.Default.TriggersExecutionMode == TriggerExecutionMode.Parallel)
                {
                    Task[] tasks = this.subscribers[typeof(TTrigger)]
                                    .OrderByDescending(t => t.Priority)
                                    .Select(t => Task.Run(() =>
                                    {
                                        t.Execute(trigger);
                                    })).ToArray();
                    Task.WaitAll(tasks); // wait for all triggers to complete
                }
                else
                {
                    Task tasks = this.subscribers[typeof(TTrigger)]
                        .OrderByDescending(t => t.Priority)
                        .Aggregate<TriggerAction, Task>(Task.CompletedTask, (prev, next) =>
                        {
                            return prev.ContinueWith((task) =>
                            {
                                next.Execute(trigger);
                            });
                        });
                    Task.WaitAll(tasks);
                }
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
