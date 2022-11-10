using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class AvailableTriggerTypeViewModel : BaseViewModel
    {
        protected ComponentSpecification eventSpec;

        public AvailableTriggerTypeViewModel(Type TriggerEventType)
        {
            this.eventSpec = new ComponentSpecification(TriggerEventType);
        }
        public Type EventType
        {
            get => this.eventSpec.ComponentType;
        }
        public string Name
        {
            get => this.eventSpec.DisplayName;
        }
    }
}
