using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace TestPatterns
{
    [DisplayName("Test Pattern Source")]
    [SettingsImplementation(typeof(TestPatternConfig))]
    public class TestPatternSource : MediaSource
    {
        public TestPatternSource() : base()
        {
            this.Name = "Test Patterns";
        }

        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("Test Patterns", "Test Patterns")
            };
        }

        public override bool Initialize(string deviceId)
        {
            this.DeviceId = deviceId;
            this.RegisterChannel(new ImageTestPatternChannel(this));
            this.RegisterChannel(new AudioTestPatternChannel(this));
            this.RegisterChannel(new DataTestPatternChannel(this));
            this.IsInitialized = true;
            return true;
        }
    }
}
