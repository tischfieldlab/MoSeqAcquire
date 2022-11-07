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

        protected override Action<Trigger> Action =>  delegate (Trigger trigger)
        {
            var settings = this.Settings as SetSynapseModeConfig;

            this.Log.Information("About to get SynapseClient");
            var client = SynapseClient.GetClient();

            this.Log.Information("About to set Synapse Mode to {Mode}", settings.Mode);
            client.Mode = settings.Mode;

            this.Log.Information("Sleeping for 1 second");
            System.Threading.Thread.Sleep(1000);
        };
    }

    public class SetSynapseModeConfig : BaseSynapseTriggerActionConfig
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
