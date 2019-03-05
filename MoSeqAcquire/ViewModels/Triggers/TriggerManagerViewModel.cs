using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class TriggerManagerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<TriggerViewModel> triggers;
        protected TriggerViewModel selectedTrigger;

        public TriggerManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.triggers = new ObservableCollection<TriggerViewModel>();
            this.PopulateAvailableTriggerTypes();
            this.PopulateAvailableActionTypes();
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public ObservableCollection<TriggerViewModel> Triggers { get => this.triggers; }
        public TriggerViewModel SelectedTrigger
        {
            get => this.selectedTrigger;
            set => this.SetField(ref this.selectedTrigger, value);
        }

        public ReadOnlyObservableCollection<AvailableTriggerTypeViewModel> AvailableTriggerTypes { get; protected set; }
        protected void PopulateAvailableTriggerTypes()
        {
            var oc = new ObservableCollection<AvailableTriggerTypeViewModel>(ProtocolHelpers.FindTriggerTypes().Select(t => new AvailableTriggerTypeViewModel(t)));
            this.AvailableTriggerTypes = new ReadOnlyObservableCollection<AvailableTriggerTypeViewModel>(oc);
        }

        public ReadOnlyObservableCollection<AvailableActionTypeViewModel> AvailableActionTypes { get; protected set; }
        protected void PopulateAvailableActionTypes()
        {
            var oc = new ObservableCollection<AvailableActionTypeViewModel>(ProtocolHelpers.FindTriggerActions().Select(t => new AvailableActionTypeViewModel(t)));
            this.AvailableActionTypes = new ReadOnlyObservableCollection<AvailableActionTypeViewModel>(oc);
        }
    }

}
