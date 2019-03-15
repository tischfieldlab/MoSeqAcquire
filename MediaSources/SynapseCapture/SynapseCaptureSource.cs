using MoSeqAcquire.Models.Attributes;
using SynapseTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    [DisplayName("Synapse Capture Source")]
    [SettingsImplementation(typeof(SynapseCaptureConfig))]
    public class SynapseCaptureSource : MediaSource
    {
        public SynapseCaptureSource() : base()
        {
            this.Name = "Direct Show Device";
        }
        public TdtUdp Device { get; protected set; }
        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            var items = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("UDP Interface", "UDP Interface")
            };
            return items;
        }
        public override bool Initalize(string DeviceId)
        {
            this.DeviceId = DeviceId;
            this.Name = "SynapseUDP";
            this.Status = "Initializing";
            this.Device = new TdtUdp("localhost");

            this.RegisterChannel(new SynapseCaptureChannel(this));

            this.IsInitialized = true;

            return true;
        }
        public override void Start()
        {
            this.FindChannel<SynapseCaptureChannel>().Enabled = true;
            base.Start();
        }
        public override void Stop()
        {
            if (!this.IsInitialized) { return; }
            base.Stop();
            this.FindChannel<SynapseCaptureChannel>().Enabled = false;
            this.Device.Close();
        }
    }
}
