using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class TriggerManagerViewModel : BaseViewModel
    {
        protected ObservableCollection<TriggerBindingViewModel> triggers;
        protected ReadOnlyObservableCollection<TriggerBindingViewModel> ro_triggers;
        protected ListCollectionView triggersView;
        protected TriggerBindingViewModel selectedTrigger;

        public TriggerManagerViewModel()
        {
            this.triggers = new ObservableCollection<TriggerBindingViewModel>();
            this.ro_triggers = new ReadOnlyObservableCollection<TriggerBindingViewModel>(this.triggers);
            //this.SetupTriggersView();
        }

        /*protected void SetupTriggersView()
        {
            this.triggersView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.triggers);

            this.triggersView.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Descending));
            this.triggersView.IsLiveSorting = true;
            this.triggersView.LiveSortingProperties.Add("Priority");

            this.triggersView.GroupDescriptions.Add(new PropertyGroupDescription("Event"));
            this.triggersView.IsLiveGrouping = true;
            this.triggersView.LiveGroupingProperties.Add("Event");
        }*/
        //public ICollectionView TriggersView { get => this.triggersView; }

        public ReadOnlyObservableCollection<TriggerBindingViewModel> Triggers { get => this.ro_triggers; }
        


        public bool HasTriggerEvent(TriggerEventViewModel triggerEventViewModel)
        {
            return this.Triggers.Any((tbvm) => tbvm.Event == triggerEventViewModel);
        }
        public void AddTriggerEvent(TriggerEventViewModel triggerEventViewModel)
        {   
            this.AddTrigger(new TriggerBindingViewModel(triggerEventViewModel));
        }
        public void AddTrigger(ProtocolTrigger protocolTrigger)
        {
            this.AddTrigger(new TriggerBindingViewModel(protocolTrigger));
        }
        public void AddTrigger(TriggerBindingViewModel triggerBindingViewModel)
        {
            //Trigger.PropertyChanged += Trigger_PropertyChanged; //TODO
            this.triggers.Add(triggerBindingViewModel);
        }


        public TriggerBindingViewModel FindBindingForAction(TriggerActionViewModel triggerActionViewModel)
        {
            return this.triggers.First((tbvm) => tbvm.Actions.Contains(triggerActionViewModel));
        }
        public TriggerBindingViewModel FindBindingForEvent(TriggerEventViewModel triggerEventViewModel)
        {
            return this.triggers.First((tbvm) => tbvm.Event == triggerEventViewModel);
        }


        /*public TriggerBindingViewModel SelectedTrigger
        {
            get => this.selectedTrigger;
            set => this.SetField(ref this.selectedTrigger, value);
        }*/


        public void RemoveTrigger(TriggerBindingViewModel Trigger)
        {
            Trigger.Actions.ForEach((tavm) => tavm.Deregister());
            Trigger.Actions.Clear();
            Trigger.Event.Deregister();
            this.triggers.Remove(Trigger);
        }
        public void RemoveTriggerAction(TriggerActionViewModel triggerActionViewModel)
        {
            triggerActionViewModel.Deregister();
            triggerActionViewModel.Binding.Actions.Remove(triggerActionViewModel);
        }
        public void RemoveTriggers()
        {
            foreach(var trigger in this.triggers.ToList())
            {
                this.RemoveTrigger(trigger);
            }
        }
        /*public void ResetStatuses()
        {
            this.triggers.ForEach(tvm => tvm.TriggerState = TriggerState.Queued);
        }
        private void Trigger_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName.Equals("Priority") || e.PropertyName.Equals("TriggerEventName"))
                this.triggersView.Refresh();
        }*/
    }
}
