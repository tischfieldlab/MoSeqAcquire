using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public enum TriggerEventState
    {
        Active,
        Inactive,
        Faulted
    }

    public class TriggerEventViewModel : BaseViewModel
    {

        protected string name;
        protected Type eventType;
        protected TriggerEvent triggerEvent;
        protected bool isRegistered;
        protected TriggerEventState state;
        protected string stateMessage;

        public TriggerEventViewModel(Type TriggerEventType)
        {
            this.eventType = TriggerEventType;
            this.Initialize();
        }
        public TriggerEventViewModel(ProtocolTriggerEvent ProtocolTriggerEvent)
        {
            this.Name = ProtocolTriggerEvent.Name;
            this.EventType = ProtocolTriggerEvent.GetEventType();

            this.Initialize();

            //need to be setup after initialization
            this.Settings.ApplySnapshot(ProtocolTriggerEvent.Config);

        }
        protected void Initialize()
        {
            if (this.Name == null)
            {
                this.Name = App.Current.Services.GetService<TriggerManagerViewModel>().GetNextDefaultTriggerName();
            }
            this.Register();
            this.PropertyChanged += (s, e) => { this.Register(); };
        }
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public Type EventType
        {
            get => this.eventType;
            set => this.SetField(ref this.eventType, value);
        }
        public BaseConfiguration Settings { get => this.triggerEvent.Settings; }
        public TriggerEventState State
        {
            get => this.state;
            set => this.SetField(ref this.state, value);
        }
        public string StateMessage
        {
            get => this.stateMessage;
            set => this.SetField(ref this.stateMessage, value);
        }
        public void Deregister(TriggerEvent triggerEvent)
        {
            if (this.isRegistered)
            {
                this.triggerAction.TriggerExecutionStarted -= Trigger_TriggerExecutionStarted;
                this.triggerAction.TriggerExecutionFinished -= Trigger_TriggerExecutionFinished;
                this.triggerAction.TriggerFaulted -= Trigger_TriggerFaulted;
                this.State = TriggerEventState.Inactive;
                this.triggerEvent.Stop();
                this.isRegistered = false;
            }
        }
        protected void Register()
        {
            if (!this.isRegistered)
            {
                if (triggerEvent != null)
                {
                    this.triggerAction.TriggerExecutionStarted += Trigger_TriggerExecutionStarted;
                    this.triggerAction.TriggerExecutionFinished += Trigger_TriggerExecutionFinished;
                    this.triggerAction.TriggerFaulted += Trigger_TriggerFaulted;
                    this.triggerEvent.Start();
                    this.isRegistered = true;
                    this.State = TriggerEventState.Active;
                }
            }
        }
    }
}
