using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;
using SynapseTools;

namespace SynapseTriggers
{
    public abstract class SynapseTrigger : TriggerAction
    {
    }

    public abstract class SynapseTriggerConfig : TriggerConfig
    {
        public string hostname;
        public int port;

        public string Hostname
        {
            get => this.hostname;
            set => this.SetField(ref this.hostname, value);
        }
        public int Port
        {
            get => this.port;
            set => this.SetField(ref this.port, value);
        }
    }


    public class SetSynapseMode : SynapseTrigger
    {
        public SetSynapseMode()
        {
            this.Config = new SetSynapseModeConfig();
        }
        public override Action<Trigger> Action
        {
            get
            {
                return delegate(Trigger trigger) 
                {
                    var settings = this.Config as SetSynapseModeConfig;
                    var client = SynapseClient.GetClient();
                    client.Mode = settings.Mode;
                };
            }
        }
    }
    public class SetSynapseModeConfig : SynapseTriggerConfig
    {
        public SynapseMode mode;

        public SynapseMode Mode
        {
            get => this.mode;
            set => this.SetField(ref this.mode, value);
        }
    }


}
