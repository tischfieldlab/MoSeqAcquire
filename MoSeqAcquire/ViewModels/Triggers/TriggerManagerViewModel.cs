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
        protected ObservableCollection<TriggerViewModel> triggers;
        protected ReadOnlyObservableCollection<TriggerViewModel> ro_triggers;
        protected ListCollectionView triggersView;
        protected TriggerViewModel selectedTrigger;

        public TriggerManagerViewModel()
        {
            this.triggers = new ObservableCollection<TriggerViewModel>();
            this.ro_triggers = new ReadOnlyObservableCollection<TriggerViewModel>(this.triggers);
            this.SetupTriggersView();
        }

        protected void SetupTriggersView()
        {
            this.triggersView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.triggers);

            this.triggersView.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Descending));
            this.triggersView.IsLiveSorting = true;
            this.triggersView.LiveSortingProperties.Add("Priority");

            this.triggersView.GroupDescriptions.Add(new PropertyGroupDescription("TriggerEventName"));
            this.triggersView.IsLiveGrouping = true;
            this.triggersView.LiveGroupingProperties.Add("TriggerEventName");
        }

        public ReadOnlyObservableCollection<TriggerViewModel> Triggers { get => this.ro_triggers; }
        public ICollectionView TriggersView { get => this.triggersView; }

        public TriggerViewModel SelectedTrigger
        {
            get => this.selectedTrigger;
            set => this.SetField(ref this.selectedTrigger, value);
        }

        public void AddTrigger(TriggerViewModel Trigger)
        {
            Trigger.PropertyChanged += Trigger_PropertyChanged;
            this.triggers.Add(Trigger);
        }

        public void AddTrigger(ProtocolTrigger ProtocolTrigger)
        {
            this.AddTrigger(new TriggerViewModel(this.Root, ProtocolTrigger));
        }

        public string GetNextDefaultTriggerName()
        {
            string namebase = "Trigger";
            string realname = "";
            int count = 1;
            while (count < int.MaxValue)
            {
                realname = namebase + "_" + count.ToString();
                if (this.triggers.Count(rvm => rvm.Name.Equals(realname)) == 0)
                {
                    break;
                }
                count++;
            }
            return realname;
        }

        public void RemoveTrigger(TriggerViewModel Trigger)
        {
            Trigger.DeregisterTrigger();
            Trigger.PropertyChanged -= this.Trigger_PropertyChanged;
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
        private void Trigger_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName.Equals("Priority") || e.PropertyName.Equals("TriggerEventName"))
                this.triggersView.Refresh();
        }
    }
}
