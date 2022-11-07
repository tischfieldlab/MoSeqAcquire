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
        public int Priority { get; set; }
        public TriggerActionConfig Config { get; protected set; }
        protected abstract Action<TriggerEvent> Action { get; }

        public void Execute(TriggerEvent Trigger)
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
    [Hidden]
    public class ActionTriggerAction : TriggerAction
    {
        public Action<TriggerEvent> Delegate { get; set; }
        protected override Action<TriggerEvent> Action => this.Delegate;
    }
}
