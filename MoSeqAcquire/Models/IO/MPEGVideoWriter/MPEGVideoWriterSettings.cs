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
        public MPEGVideoWriterSettings() : base() { }

        protected VideoCodec videoCodec;

        public VideoCodec VideoCodec
        {
            get => this.videoCodec;
            set => this.SetField(ref this.videoCodec, value);
        }
    }
}
