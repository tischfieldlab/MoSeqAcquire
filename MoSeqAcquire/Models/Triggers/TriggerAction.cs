using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;

namespace MoSeqAcquire.Models.Triggers
{
    public abstract class TriggerAction : Component
    {
        public event EventHandler<TriggerLifetimeEventArgs> TriggerExecutionStarted;
        public event EventHandler<TriggerLifetimeEventArgs> TriggerExecutionFinished;
        public event EventHandler<TriggerFaultedEventArgs> TriggerFaulted;

        public bool IsCritical { get; set; }
        public TriggerConfig Config { get; protected set; }
        protected abstract Action<Trigger> Action { get; }

        public void Execute(Trigger Trigger)
        {
            this.TriggerExecutionStarted?.Invoke(this, new TriggerLifetimeEventArgs() { Trigger = Trigger });
            try
            {
                this.Action.Invoke(Trigger);
                this.TriggerExecutionFinished?.Invoke(this, new TriggerLifetimeEventArgs() { Trigger = Trigger });
            }
            catch(Exception e)
            {
                this.TriggerFaulted?.Invoke(this, new TriggerFaultedEventArgs() { Trigger = Trigger, Exception = e });
            }
        }
    }

    [SettingsImplementation(typeof(TriggerConfig))]
    public class ActionTriggerAction : TriggerAction
    {
        public Action<Trigger> Delegate { get; set; }
        protected override Action<Trigger> Action => this.Delegate;
    }
}
