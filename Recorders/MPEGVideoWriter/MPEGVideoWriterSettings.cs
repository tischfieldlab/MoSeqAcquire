﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Recording.MPEGVideoWriter
{
    public class MPEGVideoWriterSettings : RecorderSettings
    {
        public MPEGVideoWriterSettings() : base() { }


        protected bool writeTimestamps;
        protected VideoCodec videoCodec;
        protected int videoBitrate;
        protected AudioCodec audioCodec;
        protected int audioBitrate;

        [DefaultValue(true)]
        public bool WriteTimestamps
        {
            get => this.writeTimestamps;
            set => this.SetField(ref this.writeTimestamps, value);
        }

        [DefaultValue(VideoCodec.MPEG4)]
        public VideoCodec VideoCodec
        {
            get => this.videoCodec;
            set => this.SetField(ref this.videoCodec, value);
        }
        [DefaultValue(4000000)]
        public int VideoBitrate
        {
            get => this.videoBitrate;
            set => this.SetField(ref this.videoBitrate, value);
        }
        [DefaultValue(AudioCodec.AAC)]
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
