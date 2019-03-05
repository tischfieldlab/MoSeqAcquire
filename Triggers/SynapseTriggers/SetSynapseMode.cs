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
    public class SetSynapseMode : BaseSynapseTrigger
    {
        public SetSynapseMode()
        {
            this.Config = new SetSynapseModeConfig();
        }
        public override Action<Trigger> Action
        {
            get
            {
                return delegate (Trigger trigger)
                {
                    var settings = this.Config as SetSynapseModeConfig;
                    var client = SynapseClient.GetClient();
                    client.Mode = settings.Mode;
                };
            }
        }
    }
    public class SetSynapseModeConfig : BaseSynapseTriggerConfig
    {
        protected SynapseMode mode;

        [DefaultValue(SynapseMode.Idle)]
        public SynapseMode Mode
        {
            get => this.mode;
            set => this.SetField(ref this.mode, value);
        }
    }
}
