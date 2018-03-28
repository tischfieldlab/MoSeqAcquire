using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.IO.RawDataWriter
{
    [KnownType(typeof(RecorderSettings))]
    [DisplayName("Raw Data Writer")]
    [SettingsImplementation(typeof(RecorderSettings))]
    public class RawDataWriter : MediaWriter<RawDataSink>
    {
        public RawDataWriter() : base()
        {
            
        }
        public override void ConnectChannel(Channel Channel)
        {
            this.sinks.Add(new RawDataSink(this.Settings, Channel));
        }

        public override IEnumerable<string> ListDestinations()
        {
            return this.sinks.Select(s => s.FilePath);
        }


    }

    
}
