using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Triggers;
using TriggerAction = MoSeqAcquire.Models.Triggers.TriggerAction;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public enum TriggerActionState
    {
        None,
        Queued,
        Running,
        Completed,
        Faulted
    }
    public class TriggerActionViewModel : BaseViewModel
    {
        protected TriggerBus triggerBus;
        protected string name;
        protected Type actionType;
        protected TriggerAction triggerAction;
        protected bool isRegistered;
        protected TriggerActionState triggerState;
        protected string triggerStateMessage;

        public TriggerActionViewModel(Type TriggerEventType)
        {
            this.actionType = TriggerEventType;
            this.Initialize();
        }
        public TriggerActionViewModel(ProtocolTriggerEvent ProtocolTriggerEvent)
        {
            this.Name = ProtocolTriggerEvent.Name;
            this.actionType = ProtocolTriggerEvent.GetEventType();

            this.Initialize();

            //need to be setup after initialization
            this.Settings.ApplySnapshot(ProtocolTriggerEvent.Config);

        }
        protected void Initialize()
        {
            this.triggerAction = (TriggerAction)Activator.CreateInstance(this.actionType);
            if (this.Name == null)
            {
                this.Name = this.Specification.DisplayName;
            }
            this.triggerBus = App.Current.Services.GetService<TriggerBus>();
            //this.Register();
            //this.PropertyChanged += (s, e) => { this.RegisterTrigger(); };
        }
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public bool IsCritical
        {
            get => this.triggerAction.IsCritical;
            set
            {
                this.triggerAction.IsCritical = value;
                this.NotifyPropertyChanged(nameof(this.IsCritical));
            }
        }
        public int Priority
        {
            get => this.triggerAction.Priority;
            set
            {
                this.triggerAction.Priority = value;
                this.NotifyPropertyChanged(nameof(this.Priority));
            }
        }
        public BaseConfiguration Settings { get => this.triggerAction.Settings; }
        public TriggerItemSpecification Specification
        {
            get => this.triggerAction.Specification as TriggerItemSpecification;
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
        public TriggerActionState State
        {
            get => this.triggerState;
            set => this.SetField(ref this.triggerState, value);
        }
        public string TriggerStateMessage
        {
            get => this.triggerStateMessage;
            set => this.SetField(ref this.triggerStateMessage, value);
        }


        public void Deregister(TriggerEvent triggerEvent)
        {
            if (this.isRegistered)
            {
                this.triggerAction.ExecutionStarted -= Trigger_TriggerExecutionStarted;
                this.triggerAction.ExecutionFinished -= Trigger_TriggerExecutionFinished;
                this.triggerAction.ExecutionFaulted -= Trigger_TriggerFaulted;
                this.State = TriggerActionState.None;
                this.triggerBus.Unsubscribe(triggerEvent, this.triggerAction);
                this.isRegistered = false;
            }
        }
        protected void Register(TriggerEvent triggerEvent)
        {
            if (!this.isRegistered)
            {
                if (triggerEvent != null && this.actionType != null)
                {
                    this.triggerAction.ExecutionStarted += Trigger_TriggerExecutionStarted;
                    this.triggerAction.ExecutionFinished += Trigger_TriggerExecutionFinished;
                    this.triggerAction.ExecutionFaulted += Trigger_TriggerFaulted;
                    this.triggerBus.Subscribe(triggerEvent, this.triggerAction);
                    this.isRegistered = true;
                    this.State = TriggerActionState.Queued;
                }
            }
        }

        private void Trigger_TriggerExecutionFinished(object sender, TriggerFinishedEventArgs e)
        {
            this.TriggerStateMessage = e.Output;
            this.State = TriggerActionState.Completed;
        }

        private void Trigger_TriggerExecutionStarted(object sender, TriggerLifetimeEventArgs e)
        {
            this.TriggerStateMessage = string.Empty;
            this.State = TriggerActionState.Running;
        }
        private void Trigger_TriggerFaulted(object sender, TriggerFaultedEventArgs e)
        {
            this.TriggerStateMessage = e.Output;
            //this.TriggerStateMessage = e.Exception.GetAllMessages();
            this.State = TriggerActionState.Faulted;
            if (this.IsCritical)
            {
                App.Current.Services.GetService<RecordingManagerViewModel>().AbortRecording();
                MessageBox.Show("The recording was aborted because a Critical Trigger Action faulted:\n" + this.TriggerStateMessage,
                                "Recording Aborted!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }


        public ProtocolTriggerAction GetDefinition()
        {
            return new ProtocolTriggerAction()
            {
                Name = this.Name,
                Action = this.actionType.AssemblyQualifiedName,
                Critical = this.IsCritical,
                Priority = this.Priority,
                Config = this.Settings.GetSnapshot(),
            };
        }
    }
}
