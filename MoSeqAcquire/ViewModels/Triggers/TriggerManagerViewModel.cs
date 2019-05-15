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
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<TriggerViewModel> triggers;
        protected ReadOnlyObservableCollection<TriggerViewModel> ro_triggers;
        protected TriggerViewModel selectedTrigger;

        public TriggerManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.triggers = new ObservableCollection<TriggerViewModel>();
            this.ro_triggers = new ReadOnlyObservableCollection<TriggerViewModel>(this.triggers);
            this.TriggersView = CollectionViewSource.GetDefaultView(this.triggers);
            this.TriggersView.GroupDescriptions.Add(new PropertyGroupDescription("TriggerEventName"));
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public ReadOnlyObservableCollection<TriggerViewModel> Triggers { get => this.ro_triggers; }
        public ICollectionView TriggersView { get; protected set; }

        public TriggerViewModel SelectedTrigger
        {
            get => this.selectedTrigger;
            set => this.SetField(ref this.selectedTrigger, value);
        }

        public void AddTrigger(TriggerViewModel Trigger)
        {
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
    }
}
