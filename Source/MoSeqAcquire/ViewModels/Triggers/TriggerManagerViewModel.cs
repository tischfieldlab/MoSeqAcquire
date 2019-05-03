using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class TriggerManagerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<TriggerViewModel> triggers;
        protected ReadOnlyObservableCollection<TriggerViewModel> ro_triggers;
        protected TriggerViewModel selectedTrigger;

        public TriggerManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.triggers = new ObservableCollection<TriggerViewModel>();
            this.ro_triggers = new ReadOnlyObservableCollection<TriggerViewModel>(this.triggers);
            this.PopulateAvailableTypes();
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public ReadOnlyObservableCollection<TriggerViewModel> Triggers { get => this.ro_triggers; }
        public TriggerViewModel SelectedTrigger
        {
            get => this.selectedTrigger;
            set => this.SetField(ref this.selectedTrigger, value);
        }

        public void AddTrigger()
        {
            var vm = new TriggerViewModel(this.Root);
            //vm.PropertyChanged += trigger_PropertyChanged;
            this.triggers.Add(new TriggerViewModel(this.Root));
        }
        public void AddTrigger(ProtocolTrigger ProtocolTrigger)
        {
            var vm = new TriggerViewModel(this.Root)
            {
                Name = ProtocolTrigger.Name,
                ActionType = ProtocolTrigger.GetActionType(),
                TriggerType = ProtocolTrigger.GetEventType(),
                IsCritical = ProtocolTrigger.Critical,
            };
            vm.Settings.ApplySnapshot(ProtocolTrigger.Config);
            this.triggers.Add(vm);
        }

        public void RemoveTrigger(TriggerViewModel Trigger)
        {
            this.triggers.Remove(Trigger);
        }
        public void RemoveTriggers()
        {
            this.triggers.ForEach(tvm => tvm.DeregisterTrigger());
            this.triggers.Clear();
        }
        public void ResetStatuses()
        {
            this.triggers.ForEach(tvm => tvm.TriggerState = TriggerState.Queued);
        }


        

        public ReadOnlyObservableCollection<AvailableTriggerTypeViewModel> AvailableTriggerTypes { get; protected set; }
        public ReadOnlyObservableCollection<AvailableActionTypeViewModel> AvailableActionTypes { get; protected set; }
        protected void PopulateAvailableTypes()
        {
            var oc1 = new ObservableCollection<AvailableTriggerTypeViewModel>(ProtocolHelpers.FindTriggerTypes().Select(t => new AvailableTriggerTypeViewModel(t)));
            this.AvailableTriggerTypes = new ReadOnlyObservableCollection<AvailableTriggerTypeViewModel>(oc1);

            var oc2 = new ObservableCollection<AvailableActionTypeViewModel>(ProtocolHelpers.FindTriggerActions().Select(t => new AvailableActionTypeViewModel(t.ComponentType)));
            this.AvailableActionTypes = new ReadOnlyObservableCollection<AvailableActionTypeViewModel>(oc2);
        }

        

    }

}
