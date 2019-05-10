using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;
using Serilog;
using Serilog.Core;

namespace MoSeqAcquire.Models.Triggers
{
    public abstract class TriggerAction : Component
    {
        public event EventHandler<TriggerLifetimeEventArgs> TriggerExecutionStarted;
        public event EventHandler<TriggerFinishedEventArgs> TriggerExecutionFinished;
        public event EventHandler<TriggerFaultedEventArgs> TriggerFaulted;

        public TriggerAction() : base()
        {
            this.Specification = new TriggerActionSpecification(this.GetType());
            this.Settings = this.Specification.SettingsFactory();

            this.Output = new StringWriter();
            this.Log = new LoggerConfiguration()
                .WriteTo.TextWriter(this.Output)
                .WriteTo.Sink((ILogEventSink)Serilog.Log.Logger)
                //.Enrich.WithProperty("Trigger", Trigger)
                //.Enrich.WithProperty("Action", this)
                .CreateLogger();
        }

        public bool IsCritical { get; set; }
        public int Priority { get; set; }
        protected ILogger Log { get; private set; }
        protected StringWriter Output { get; private set; }
        protected abstract Action<Trigger> Action { get; }


        public void Execute(Trigger Trigger)
        {
            this.Output.GetStringBuilder().Clear();
            this.Log.Information("Starting Execution of {TriggerAction} for Trigger {Event}", this, Trigger);
            this.TriggerExecutionStarted?.Invoke(this, new TriggerLifetimeEventArgs() { Trigger = Trigger });
            try
            {
                this.Action.Invoke(Trigger);
                this.TriggerExecutionFinished?.Invoke(this, new TriggerFinishedEventArgs() {Trigger = Trigger, Output = this.Output.ToString() });
            }
            catch (Exception e)
            {
                this.Log.Error(e, "Error during trigger action execution");
                this.TriggerFaulted?.Invoke(this, new TriggerFaultedEventArgs()
                {
                    Trigger = Trigger,
                    Exception = e,
                    Output = this.Output.ToString()
                });
            }
            this.Log.Information("Completed Execution of {TriggerAction} for Trigger {Event}", this, Trigger);
        }
    }

    [SettingsImplementation(typeof(TriggerConfig))]
    [Hidden]
    public class ActionTriggerAction : TriggerAction
    {
        public Action<Trigger> Delegate { get; set; }
        protected override Action<Trigger> Action => this.Delegate;
    }
}
