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
    [SettingsImplementation(typeof(TriggerEventConfig))]
    public abstract class TriggerEvent : Component
    {
        public event EventHandler<TriggerLifetimeEventArgs> ExecutionStarted;
        public event EventHandler<TriggerFinishedEventArgs> ExecutionFinished;
        public event EventHandler<TriggerFaultedEventArgs> ExecutionFaulted;


        public TriggerEvent()
        {
            this.Specification = new TriggerItemSpecification(this.GetType());
            this.Settings = this.Specification.SettingsFactory();
        }

        public TriggerEventConfig Config { get; protected set; }

        public void Fire()
        {
            App.Current.Services.GetService<TriggerBus>().Trigger(this);
        }


        public abstract void Start();

        public abstract void Stop();

    }

    

}
