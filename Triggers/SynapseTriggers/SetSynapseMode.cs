using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;
using SynapseTools;

namespace SynapseTriggers
{
    [DisplayName("Synapse Set Mode")]
    [SettingsImplementation(typeof(SetSynapseModeConfig))]
    public class SetSynapseMode : BaseSynapseTrigger
    {
        public SetSynapseMode() : base()
        {
            
        }
        protected override Action<Trigger> Action
        {
            get
            {
                return delegate (Trigger trigger)
                {
                    var settings = this.Settings as SetSynapseModeConfig;
                    var client = SynapseClient.GetClient();
                    client.Mode = settings.Mode;
                    System.Threading.Thread.Sleep(1000);
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
