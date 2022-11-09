using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    [SettingsImplementation(typeof(TriggerEventConfig))]
    public abstract class TriggerEvent : Component
    {
        public TriggerEvent()
        {
            this.Specification = new TriggerActionSpecification(this.GetType());
            this.Settings = this.Specification.SettingsFactory();
        }

        public TriggerEventConfig Config { get; protected set; }


        public abstract void Start();

        public abstract void Stop();

    }

    

}
