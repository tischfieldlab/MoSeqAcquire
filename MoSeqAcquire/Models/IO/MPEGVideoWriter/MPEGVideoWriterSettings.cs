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
        protected int videoBitrate;
        protected AudioCodec audioCodec;
        protected int audioBitrate;

        public VideoCodec VideoCodec
        {
            get => this.videoCodec;
            set => this.SetField(ref this.videoCodec, value);
        }
        public int VideoBitrate
        {
            get => this.videoBitrate;
            set => this.SetField(ref this.videoBitrate, value);
        }

        public AudioCodec AudioCodec
        {
            get => this.audioCodec;
            set => this.SetField(ref this.audioCodec, value);
        }
        public int AudioBitrate
        {
            get => this.audioBitrate;
            set => this.SetField(ref this.audioBitrate, value);
        }
    }
}
