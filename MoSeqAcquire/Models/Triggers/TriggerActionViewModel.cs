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

namespace MoSeqAcquire.Models.Triggers
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
        public TriggerActionSpecification Specification
        {
            get => this.triggerAction.Specification as TriggerActionSpecification;
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
        public TriggerActionState TriggerState
        {
            get => this.triggerState;
            set => this.SetField(ref this.triggerState, value);
        }
        public string TriggerStateMessage
        {
            get => this.triggerStateMessage;
            set => this.SetField(ref this.triggerStateMessage, value);
        }


        public void DeregisterTrigger(TriggerEvent triggerEvent)
        {
            if (this.isRegistered)
            {
                this.triggerAction.TriggerExecutionStarted -= Trigger_TriggerExecutionStarted;
                this.triggerAction.TriggerExecutionFinished -= Trigger_TriggerExecutionFinished;
                this.triggerAction.TriggerFaulted -= Trigger_TriggerFaulted;
                this.TriggerState = TriggerActionState.None;
                this.triggerBus.Unsubscribe(triggerEvent, this.triggerAction);
                this.isRegistered = false;
            }
        }
        protected void RegisterTrigger(TriggerEvent triggerEvent)
        {
            if (!this.isRegistered)
            {
                if (triggerEvent != null && this.actionType != null)
                {
                    this.triggerAction.TriggerExecutionStarted += Trigger_TriggerExecutionStarted;
                    this.triggerAction.TriggerExecutionFinished += Trigger_TriggerExecutionFinished;
                    this.triggerAction.TriggerFaulted += Trigger_TriggerFaulted;
                    this.triggerBus.Subscribe(triggerEvent, this.triggerAction);
                    this.isRegistered = true;
                    this.TriggerState = TriggerActionState.Queued;
                }
            }
        }

        private void Trigger_TriggerExecutionFinished(object sender, TriggerFinishedEventArgs e)
        {
            this.TriggerStateMessage = e.Output;
            this.TriggerState = TriggerActionState.Completed;
        }

        private void Trigger_TriggerExecutionStarted(object sender, TriggerLifetimeEventArgs e)
        {
            this.TriggerStateMessage = string.Empty;
            this.TriggerState = TriggerActionState.Running;
        }
        private void Trigger_TriggerFaulted(object sender, TriggerFaultedEventArgs e)
        {
            this.TriggerStateMessage = e.Output;
            //this.TriggerStateMessage = e.Exception.GetAllMessages();
            this.TriggerState = TriggerActionState.Faulted;
            if (this.IsCritical)
            {
                App.Current.Services.GetService<RecordingManagerViewModel>().AbortRecording();
                MessageBox.Show("The recording was aborted because a Critical Trigger Action faulted:\n" + this.TriggerStateMessage,
                                "Recording Aborted!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }

    }
}
