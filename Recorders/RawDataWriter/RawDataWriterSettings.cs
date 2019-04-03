using System.ComponentModel;

namespace MoSeqAcquire.Models.Recording.RawDataWriter
{
    public class RawDataWriterSettings : RecorderSettings
    {
        public RawDataWriterSettings() : base() { }

        protected bool enableGzip;
        protected bool writeTimestamps;

        public bool EnableGZipCompression
        {
            get => this.enableGzip;
            set => this.SetField(ref this.enableGzip, value);
        }

        [DefaultValue(true)]
        public bool WriteTimestamps
        {
            get => this.writeTimestamps;
            set => this.SetField(ref this.writeTimestamps, value);
        }
    }
}
