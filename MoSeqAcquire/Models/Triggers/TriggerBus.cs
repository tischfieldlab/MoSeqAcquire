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
        Parallel,
        Hybrid
    }
    public class TriggerBus
    {
        protected Dictionary<TriggerEvent, List<TriggerAction>> subscribers;

        public TriggerBus()
        {
            this.subscribers = new Dictionary<TriggerEvent, List<TriggerAction>>();
        }

        public TriggerExecutionMode TriggerExecutionMode
        {
            get => (TriggerExecutionMode)Enum.Parse(typeof(TriggerExecutionMode), Properties.Settings.Default.TriggersExecutionMode);
        }
        
        public void Subscribe(TriggerEvent triggerEvent, TriggerAction triggerAction)
        {
            if (!this.subscribers.ContainsKey(triggerEvent))
            {
                this.subscribers[triggerEvent] = new List<TriggerAction>();
            }
            this.subscribers[triggerEvent].Add(triggerAction);
        }
        public void Unsubscribe(TriggerEvent triggerEvent, TriggerAction triggerAction)
        {
            this.subscribers[triggerEvent].Remove(triggerAction);
        }

        public void Trigger<TTrigger>(TTrigger triggerEvent) where TTrigger : TriggerEvent
        {
            if (this.subscribers.ContainsKey(triggerEvent))
            {
                if (this.TriggerExecutionMode == TriggerExecutionMode.Parallel)
                {
                    Task[] tasks = this.subscribers[triggerEvent]
                                    .OrderByDescending(t => t.Priority)
                                    .Select(t => Task.Run(() => t.Execute(triggerEvent)))
                                    .ToArray();
                    Task.WaitAll(tasks); // wait for all triggers to complete
                }
                else if(this.TriggerExecutionMode == TriggerExecutionMode.Hybrid)
                {
                    Task task = this.subscribers[triggerEvent]
                                       .GroupBy(t => t.Priority)
                                       .OrderByDescending(g => g.Key)
                                       .Aggregate(Task.CompletedTask, (prev, next) =>
                                       {
                                           return prev.ContinueWith((tsk) =>
                                           {
                                               Task[] groupTasks = next.Select(t => Task.Run(() =>
                                               {
                                                   t.Execute(triggerEvent);
                                               })).ToArray();

                                               Task.WaitAll(groupTasks);
                                               return Task.CompletedTask;
                                           });
                                       });
                    task.Wait();
                }
                else
                {
                    Task tasks = this.subscribers[triggerEvent]
                        .OrderByDescending(t => t.Priority)
                        .Aggregate<TriggerAction, Task>(Task.CompletedTask, (prev, next) =>
                        {
                            return prev.ContinueWith((tsk) =>
                            {
                                next.Execute(triggerEvent);
                            });
                        });
                    tasks.Wait();
                }
            }
        }
    }
}
