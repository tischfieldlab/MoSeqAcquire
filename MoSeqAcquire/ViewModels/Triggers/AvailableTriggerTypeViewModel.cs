using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class AvailableTriggerTypeViewModel : BaseViewModel
    {
        protected Type triggerType;
        public AvailableTriggerTypeViewModel(Type TriggerType)
        {
            this.triggerType = TriggerType;
        }
        public Type TriggerType
        {
            get => this.triggerType;
        }
        public string Name
        {
            get => (Activator.CreateInstance(this.triggerType) as TriggerEvent).Name;
        }
    }
}
