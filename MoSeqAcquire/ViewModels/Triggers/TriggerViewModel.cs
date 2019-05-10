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
using Trigger = MoSeqAcquire.Models.Triggers.Trigger;
using TriggerAction = MoSeqAcquire.Models.Triggers.TriggerAction;

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
        protected string triggerStateMessage;


        public TriggerViewModel(MoSeqAcquireViewModel RootViewModel, Type TriggerActionType)
        {
            this.rootViewModel = RootViewModel;
            this.actionType = TriggerActionType;
            this.Initialize();
        }
        public TriggerViewModel(MoSeqAcquireViewModel RootViewModel, ProtocolTrigger ProtocolTrigger)
        {
            this.rootViewModel = RootViewModel;
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
                this.Name = this.rootViewModel.Triggers.GetNextDefaultTriggerName();
            }
            this.trigger = (TriggerAction)Activator.CreateInstance(this.actionType);
            this.RegisterTrigger();
            this.PropertyChanged += (s, e) => { this.RegisterTrigger(); };
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
                    return (Activator.CreateInstance(this.triggerType) as Trigger).Name;
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
                this.NotifyPropertyChanged();
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
                this.Root.TriggerBus.Unsubscribe(this.triggerType, this.trigger);
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
            this.TriggerStateMessage = e.Exception.GetAllMessages();
            this.TriggerState = TriggerState.Faulted;
            if(this.IsCritical)
            {
                this.Root.Recorder.AbortRecording();
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
                Event = this.TriggerType.AssemblyQualifiedName,
                Action = this.ActionType.AssemblyQualifiedName,
                Config = this.Settings.GetSnapshot()
            };
        }
    }
}
