using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Triggers;
using MoSeqAcquire.Models.Utility;
using TriggerEvent = MoSeqAcquire.Models.Triggers.TriggerEvent;
using TriggerAction = MoSeqAcquire.Models.Triggers.TriggerAction;
using Microsoft.Extensions.DependencyInjection;
using MoSeqAcquire.ViewModels.Recording;

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
        protected TriggerBus triggerBus;
        protected string name;
        protected Type triggerType;
        protected Type actionType;
        protected TriggerAction trigger;
        protected bool isRegistered;
        protected TriggerState triggerState;
        protected string triggerStateMessage;

        public TriggerViewModel(Type TriggerActionType)
        {
            this.actionType = TriggerActionType;
            this.Initialize();
        }
        public TriggerViewModel(ProtocolTrigger ProtocolTrigger)
        {
            this.Name = ProtocolTrigger.Name;
            this.actionType = ProtocolTrigger.GetActionType();
            this.TriggerType = ProtocolTrigger.GetEventType();
            
            this.Initialize();

            //need to be setup after initialization
            this.IsCritical = ProtocolTrigger.Critical;
            this.Settings.ApplySnapshot(ProtocolTrigger.Config);

        }

        protected void Initialize()
        {
            if (this.Name == null)
            {
                this.Name = App.Current.Services.GetService<TriggerManagerViewModel>().GetNextDefaultTriggerName();
            }
            this.triggerBus = App.Current.Services.GetService<TriggerBus>();
            this.trigger = (TriggerAction)Activator.CreateInstance(this.actionType);
            this.RegisterTrigger();
            this.PropertyChanged += (s, e) => { this.RegisterTrigger(); };
        }

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
        }

        public string TriggerActionName
        {
            get => this.trigger.Specification.DisplayName;
        }

        public string TriggerEventName
        {
            get
            {
                if (this.triggerType != null)
                {
                    return (Activator.CreateInstance(this.triggerType) as TriggerEvent).Name;
                }
                return "None selected";
            }
        }

        public bool IsCritical
        {
            get => this.trigger.IsCritical;
            set
            {
                this.trigger.IsCritical = value;
                this.NotifyPropertyChanged(nameof(this.IsCritical));
            }
        }
        public int Priority
        {
            get => this.trigger.Priority;
            set
            {
                this.trigger.Priority = value;
                this.NotifyPropertyChanged(nameof(this.Priority));
            }
        }
        public BaseConfiguration Settings { get => this.trigger.Settings; }
        public TriggerActionSpecification Specification
        {
            get => this.trigger.Specification as TriggerActionSpecification;
        }

        public bool HasDesignerImplementation
        {
            get => this.Specification.HasDesignerImplementation;
        }
        public UserControl DesignerImplementation
        {
            get
            {
                if (this.Specification.HasDesignerImplementation)
                    return (UserControl)Activator.CreateInstance(this.Specification.DesignerImplementation);
                return null;
            }
        }
        public TriggerState TriggerState
        {
            get => this.triggerState;
            set => this.SetField(ref this.triggerState, value);
        }
        public string TriggerStateMessage
        {
            get => this.triggerStateMessage;
            set => this.SetField(ref this.triggerStateMessage, value);
        }

        public void DeregisterTrigger()
        {
            if (this.isRegistered)
            {
                this.trigger.TriggerExecutionStarted -= Trigger_TriggerExecutionStarted;
                this.trigger.TriggerExecutionFinished -= Trigger_TriggerExecutionFinished;
                this.trigger.TriggerFaulted -= Trigger_TriggerFaulted;
                this.TriggerState = TriggerState.None;
                this.triggerBus.Unsubscribe(this.triggerType, this.trigger);
                this.isRegistered = false;
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
                    this.triggerBus.Subscribe(this.triggerType, this.trigger);
                    this.isRegistered = true;
                    this.TriggerState = TriggerState.Queued;
                }
            }
        }

        private void Trigger_TriggerExecutionFinished(object sender, TriggerFinishedEventArgs e)
        {
            this.TriggerStateMessage = e.Output;
            this.TriggerState = TriggerState.Completed;
        }

        private void Trigger_TriggerExecutionStarted(object sender, TriggerLifetimeEventArgs e)
        {
            this.TriggerStateMessage = string.Empty;
            this.TriggerState = TriggerState.Running;
        }
        private void Trigger_TriggerFaulted(object sender, TriggerFaultedEventArgs e)
        {
            this.TriggerStateMessage = e.Output;
            //this.TriggerStateMessage = e.Exception.GetAllMessages();
            this.TriggerState = TriggerState.Faulted;
            if(this.IsCritical)
            {
                App.Current.Services.GetService<RecordingManagerViewModel>().AbortRecording();
                MessageBox.Show("The recording was aborted because a Critical Trigger Action faulted:\n" + this.TriggerStateMessage,
                                "Recording Aborted!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }

        public ProtocolTrigger GetTriggerDefinition()
        {
            return new ProtocolTrigger()
            {
                Name = this.Name,
                Critical = this.IsCritical,
                Priority = this.Priority,
                Event = this.TriggerType.AssemblyQualifiedName,
                EventConfig = this.Settings.GetSnapshot(),
                Action = this.ActionType.AssemblyQualifiedName,
                ActionConfig = this.Settings.GetSnapshot()
            };
        }
    }
}
