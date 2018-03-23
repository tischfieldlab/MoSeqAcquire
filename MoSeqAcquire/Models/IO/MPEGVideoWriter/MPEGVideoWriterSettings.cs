using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.IO.MPEGVideoWriter
{
    public class MPEGVideoWriterSettings : RecorderSettings
    {
        public MPEGVideoWriterSettings(RecorderSettings ParentSettings) : base(ParentSettings)
        {

        }
        public VideoCodec VideoCodec { get; set; }

        public override void ApplySnapshot(ConfigSnapshot snapshot)
        {
            base.ApplySnapshot(snapshot);
            var ss = snapshot as MPEGVideoWriterSettings;
            this.VideoCodec = ss.VideoCodec;
        }
    }
}
