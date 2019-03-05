using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class TriggerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected Type triggerType;
        protected Type actionType;
        protected TriggerAction trigger;


        public TriggerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.PropertyChanged += TriggerViewModel_PropertyChanged;
        }

        private void TriggerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ActionType"))
            {
                this.trigger = (TriggerAction)Activator.CreateInstance(this.actionType);
            }
        }

        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public string DisplayName { get; }
        public Type TriggerType
        {
            get => this.triggerType;
            set => this.SetField(ref this.triggerType, value);
        }
        public Type ActionType
        {
            get => this.actionType;
            set => this.SetField(ref this.actionType, value);
        }
        public TriggerConfig Settings { get => this.trigger.Config; }
    }
}
