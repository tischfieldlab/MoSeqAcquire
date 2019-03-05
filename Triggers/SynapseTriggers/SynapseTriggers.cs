using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;
using SynapseTools;

namespace SynapseTriggers
{
    public abstract class BaseSynapseTrigger : TriggerAction
    {
    }

    public abstract class BaseSynapseTriggerConfig : TriggerConfig
    {
        public string hostname;
        public int port;

        [DefaultValue("localhost")]
        //[DefaultValue("10.1.0.55")] //this is the real address 
        public string Hostname
        {
            get => this.hostname;
            set => this.SetField(ref this.hostname, value);
        }
        [DefaultValue(24414)]
        public int Port
        {
            get => this.port;
            set => this.SetField(ref this.port, value);
        }
    }
}
