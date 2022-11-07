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
        protected Dictionary<Type, List<TriggerAction>> subscribers;

        public TriggerBus()
        {
            this.subscribers = new Dictionary<Type, List<TriggerAction>>();
        }

        public TriggerExecutionMode TriggerExecutionMode
        {
            get => (TriggerExecutionMode)Enum.Parse(typeof(TriggerExecutionMode), Properties.Settings.Default.TriggersExecutionMode);
        }
        
        public void Subscribe(Type trigger, TriggerAction triggerAction)
        {
            if (!this.subscribers.ContainsKey(trigger))
            {
                this.subscribers[trigger] = new List<TriggerAction>();
            }
            this.subscribers[trigger].Add(triggerAction);
        }
        public void Unsubscribe(Type trigger, TriggerAction triggerAction)
        {
            this.subscribers[trigger].Remove(triggerAction);
        }

        public void Trigger<TTrigger>(TTrigger trigger) where TTrigger : TriggerEvent
        {
            if (this.subscribers.ContainsKey(typeof(TTrigger)))
            {
                if (this.TriggerExecutionMode == TriggerExecutionMode.Parallel)
                {
                    Task[] tasks = this.subscribers[typeof(TTrigger)]
                                    .OrderByDescending(t => t.Priority)
                                    .Select(t => Task.Run(() =>
                                    {
                                        t.Execute(trigger);
                                    })).ToArray();
                    Task.WaitAll(tasks); // wait for all triggers to complete
                }
                else if(this.TriggerExecutionMode == TriggerExecutionMode.Hybrid)
                {
                    Task task = this.subscribers[typeof(TTrigger)]
                                       .GroupBy(t => t.Priority)
                                       .OrderByDescending(g => g.Key)
                                       .Aggregate(Task.CompletedTask, (prev, next) =>
                                       {
                                           return prev.ContinueWith((tsk) =>
                                           {
                                               Task[] groupTasks = next.Select(t => Task.Run(() =>
                                               {
                                                   t.Execute(trigger);
                                               })).ToArray();

                                               Task.WaitAll(groupTasks);
                                               return Task.CompletedTask;
                                           });
                                       });
                    task.Wait();
                }
                else
                {
                    Task tasks = this.subscribers[typeof(TTrigger)]
                        .OrderByDescending(t => t.Priority)
                        .Aggregate<TriggerAction, Task>(Task.CompletedTask, (prev, next) =>
                        {
                            return prev.ContinueWith((tsk) =>
                            {
                                next.Execute(trigger);
                            });
                        });
                    tasks.Wait();
                }
            }
        }
    }
}
