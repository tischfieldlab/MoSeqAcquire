using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public enum TriggerEventState
    {
        Active,
        Inactive
    }

    public class TriggerEventViewModel : BaseViewModel
    {

        protected string name;
        protected Type eventType;
        protected TriggerEvent triggerEvent;
        protected TriggerEventState triggerState;
        protected string triggerStateMessage;


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
    }
}
