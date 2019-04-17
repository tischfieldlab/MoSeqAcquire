using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Triggers
{
    public class TriggerConfig : BaseConfiguration
    {
        public TriggerConfig() : base()
        {
            this.ApplyDefaults();
        }
    }
}
