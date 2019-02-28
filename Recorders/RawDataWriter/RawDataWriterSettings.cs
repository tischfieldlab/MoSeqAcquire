namespace MoSeqAcquire.Models.Recording.RawDataWriter
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
