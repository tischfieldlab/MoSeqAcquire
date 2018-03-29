using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.IO.RawDataWriter
{
    public class RawDataWriterSettings : RecorderSettings
    {
        public RawDataWriterSettings() : base() { }

        protected bool enableGzip;

        public bool EnableGZipCompression
        {
            get => this.enableGzip;
            set => this.SetField(ref this.enableGzip, value);
        }
    }
}
