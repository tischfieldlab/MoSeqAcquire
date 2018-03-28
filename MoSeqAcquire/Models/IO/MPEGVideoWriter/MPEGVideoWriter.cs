using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Accord.Video;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.IO.MPEGVideoWriter
{
    [KnownType(typeof(MPEGVideoWriterSettings))]
    [DisplayName("MPEG Video Writer")]
    [SettingsImplementation(typeof(MPEGVideoWriterSettings))]
    [SupportedChannelType(MediaType.Video, 1)]
    [SupportedChannelType(MediaType.Audio, 1)]
    public class MPEGVideoWriter : MediaWriter<MPEGVideoWriterSink>
    {
        public override void ConnectChannel(Channel Channel)
        {
            this.sinks.Add(new MPEGVideoWriterSink(this.Settings, Channel));
        }

        public override IEnumerable<string> ListDestinations()
        {
            throw new NotImplementedException();
        }
    }

    
}
