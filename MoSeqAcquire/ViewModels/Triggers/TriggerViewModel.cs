using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public enum TriggerState
    {
        None,
        Queued,
        Running,
        Completed,
        Faulted
    }

    public class TriggerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected string name;
        protected Type triggerType;
        protected Type actionType;
        protected TriggerAction trigger;
        protected bool isRegistered;
        protected TriggerState triggerState;


        public TriggerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.PropertyChanged += TriggerViewModel_PropertyChanged;
            this.triggerState = TriggerState.None;
        }

        private void TriggerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ActionType"))
            {
                this.trigger = (TriggerAction)Activator.CreateInstance(this.actionType);
            }
            this.RegisterTrigger();
        }

        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public Type TriggerType
        {
            get => this.triggerType;
            set => this.SetField(ref this.triggerType, value, this.DeregisterTrigger);
        }
        public Type ActionType
        {
            get => this.actionType;
            set => this.SetField(ref this.actionType, value);
        }
        public TriggerConfig Settings { get => this.trigger.Config; }
        public TriggerState TriggerState
        {
            get => this.triggerState;
            set => this.SetField(ref this.triggerState, value);
        }
        protected void DeregisterTrigger()
        {
            if (this.isRegistered)
            {
                this.trigger.TriggerExecutionStarted -= Trigger_TriggerExecutionStarted;
                this.trigger.TriggerExecutionFinished -= Trigger_TriggerExecutionFinished;
                this.trigger.TriggerFaulted -= Trigger_TriggerFaulted;
                this.Root.TriggerBus.Unsubscribe(this.triggerType, this.trigger);
                this.isRegistered = false;
                this.TriggerState = TriggerState.None;
            }
        }

        

        protected void RegisterTrigger()
        {
            if (!this.isRegistered)
            {
                if (this.triggerType != null && this.actionType != null)
                {
                    this.trigger.TriggerExecutionStarted += Trigger_TriggerExecutionStarted;
                    this.trigger.TriggerExecutionFinished += Trigger_TriggerExecutionFinished;
                    this.trigger.TriggerFaulted += Trigger_TriggerFaulted;
                    this.Root.TriggerBus.Subscribe(this.triggerType, this.trigger);
                    this.isRegistered = true;
                    this.TriggerState = TriggerState.Queued;
                }
            }
        }

        private void Trigger_TriggerExecutionFinished(object sender, TriggerLifetimeEventArgs e)
        {
            this.TriggerState = TriggerState.Completed;
        }

        private void Trigger_TriggerExecutionStarted(object sender, TriggerLifetimeEventArgs e)
        {
            this.TriggerState = TriggerState.Running;
        }
        private void Trigger_TriggerFaulted(object sender, TriggerFaultedEventArgs e)
        {
            this.TriggerState = TriggerState.Faulted;
        }
    }
}
