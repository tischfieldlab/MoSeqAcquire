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
using System.Collections.ObjectModel;

namespace MoSeqAcquire.ViewModels.Triggers
{
    
    public class TriggerBindingViewModel : BaseViewModel
    {
        protected TriggerEventViewModel triggerEvent;
        protected ObservableCollection<TriggerActionViewModel> triggerActions;

        public TriggerBindingViewModel(TriggerEventViewModel triggerEventViewModel)
        {
            this.triggerEvent = triggerEventViewModel;
            this.triggerActions = new ObservableCollection<TriggerActionViewModel>();
        }

        public TriggerEventViewModel Event { get => this.triggerEvent; }
        public ObservableCollection<TriggerActionViewModel> Actions { get => this.triggerActions; }


        public ProtocolTrigger GetDefinition()
        {
            return new ProtocolTrigger()
            {
                Event = this.triggerEvent.GetDefinition(),
                Actions = new Collection<ProtocolTriggerAction>(this.triggerActions.Select((tavm) => tavm.GetDefinition()).ToList()),
            };
        }
    }

    

    /*public class TriggerViewModel : BaseViewModel
    {
        protected TriggerBus triggerBus;
        protected string name;
        protected Type triggerType;
        protected Type actionType;
        protected TriggerEvent triggerEvent;
        protected TriggerAction triggerAction;
        protected bool isRegistered;
        protected TriggerActionState triggerState;
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
        /*
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
        }*/


        

        

        /*public ProtocolTrigger GetTriggerDefinition()
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
        }*/
   /* }*/
}
