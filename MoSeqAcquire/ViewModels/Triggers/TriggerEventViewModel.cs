﻿using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

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
            this.EventType = TriggerEventType;
            this.triggerEvent = (TriggerEvent)Activator.CreateInstance(this.eventType);
            this.Name = this.Specification.DisplayName;
            this.Initialize();
        }
        public TriggerEventViewModel(ProtocolTriggerEvent ProtocolTriggerEvent)
        {
            this.EventType = ProtocolTriggerEvent.GetEventType();
            this.triggerEvent = (TriggerEvent)Activator.CreateInstance(this.eventType);
            this.Name = ProtocolTriggerEvent.Name;
            this.Settings.ApplySnapshot(ProtocolTriggerEvent.Config);
            this.Initialize();
        }
        protected void Initialize()
        {
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
        public TriggerEvent TriggerEvent { get => this.triggerEvent; }
        public BaseConfiguration Settings { get => this.triggerEvent.Settings; }
        public TriggerItemSpecification Specification
        {
            get => this.triggerEvent.Specification as TriggerItemSpecification;
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
        public void Deregister()
        {
            if (this.isRegistered)
            {
                this.triggerEvent.ExecutionStarted -= Trigger_TriggerExecutionStarted;
                this.triggerEvent.ExecutionFinished -= Trigger_TriggerExecutionFinished;
                this.triggerEvent.ExecutionFaulted -= Trigger_TriggerFaulted;
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
                    this.triggerEvent.ExecutionStarted += Trigger_TriggerExecutionStarted;
                    this.triggerEvent.ExecutionFinished += Trigger_TriggerExecutionFinished;
                    this.triggerEvent.ExecutionFaulted += Trigger_TriggerFaulted;
                    this.triggerEvent.Start();
                    this.isRegistered = true;
                }
            }
        }
        private void Trigger_TriggerExecutionFinished(object sender, TriggerEventFinishedEventArgs e)
        {
            this.StateMessage = e.Output;
            this.State = TriggerEventState.Inactive;
        }

        private void Trigger_TriggerExecutionStarted(object sender, TriggerEventLifetimeEventArgs e)
        {
            this.StateMessage = string.Empty;
            this.State = TriggerEventState.Active;
        }
        private void Trigger_TriggerFaulted(object sender, TriggerEventFaultedEventArgs e)
        {
            this.StateMessage = e.Output;
            // this.TriggerStateMessage = e.Exception.GetAllMessages();
            this.State = TriggerEventState.Faulted;

            // App.Current.Services.GetService<RecordingManagerViewModel>().AbortRecording();
            /*MessageBox.Show("The recording was aborted because a Critical Trigger Action faulted:\n" + this.StateMessage,
                            "Recording Aborted!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);*/
        }


        public ProtocolTriggerEvent GetDefinition()
        {
            return new ProtocolTriggerEvent()
            {
                Name = this.Name,
                Event = this.eventType.AssemblyQualifiedName,
                Config = this.Settings.GetSnapshot(),
            };
        }
    }
}
