using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MoSeqAcquire.Models.Triggers
{
    public abstract class TriggerEvent : Component
    {
        public event EventHandler<TriggerEventLifetimeEventArgs> ExecutionStarted;
        public event EventHandler<TriggerEventFinishedEventArgs> ExecutionFinished;
        public event EventHandler<TriggerEventFaultedEventArgs> ExecutionFaulted;


        public TriggerEvent()
        {
            this.Specification = new TriggerItemSpecification(this.GetType());
            this.Settings = this.Specification.SettingsFactory();
        }

        public void Fire()
        {
            App.Current.Services.GetService<TriggerBus>().Trigger(this);
        }

        protected void OnExecutionStarted()
        {
            this.ExecutionStarted?.Invoke(this, new TriggerEventLifetimeEventArgs());
        }
        protected void OnExecutionFinished(string output = "")
        {
            this.ExecutionFinished?.Invoke(this, new TriggerEventFinishedEventArgs() { Output = output });
        }
        protected void OnExecutionFaulted(Exception exception, string output = "")
        {
            this.ExecutionFaulted?.Invoke(this, new TriggerEventFaultedEventArgs()
            {
                Output = output,
                Exception = exception,
            });
        }


        public abstract void Start();

        public abstract void Stop();

    }
}
