using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Triggers
{
    public abstract class TriggerConfig : BaseConfiguration
    {
        public TriggerConfig() : base()
        {
            this.ApplyDefaults();
        }
    }

    public abstract class TriggerAction
    {
        public TriggerConfig Config { get; protected set; }
        public abstract Action<Trigger> Action { get; }
    }
}
