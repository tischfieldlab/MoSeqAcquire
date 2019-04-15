using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Recording.TextDataWriter
{
    [DisplayName("Plain Text Data Writer")]
    [SettingsImplementation(typeof(TextDataWriterSettings))]
    [SupportedChannelType(MediaType.Any, ChannelCapacity.Single)]
    public class TextDataWriter : MediaWriter
    {
        protected MediaWriterPin dataPin;

        protected FileStream file;
        protected GZipStream compressor;
        protected StreamWriter writer;
        protected TimestampCoWriter tsWriter;

        public TextDataWriter() : base()
        {
            this.dataPin = new MediaWriterPin(MediaType.Any, ChannelCapacity.Single, this.GetActionBlock);
            this.RegisterPin(this.dataPin);
        }
        
        protected override string Ext
        {
            get
            {
                string ext = "txt";
                if ((this.Settings as TextDataWriterSettings).EnableGZipCompression)
                {
                    ext += ".gz";
                }
                return ext;
            }
        }

        public override void Start()
        {
            var cfg = this.Settings as TextDataWriterSettings;

            this.file = File.Open(this.FilePath, FileMode.Create);
            if (cfg.EnableGZipCompression)
            {
                this.compressor = new GZipStream(this.file, CompressionMode.Compress);
                this.writer = new StreamWriter(this.compressor);
            }
            else
            {
                this.writer = new StreamWriter(this.file);
            }

            if (cfg.WriteTimestamps)
            {
                this.tsWriter = new TimestampCoWriter(this.FormatFilePath("{0}_ts.txt"));
                this.tsWriter.Open();
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            this.writer.Close();
            if (this.tsWriter != null)
            {
                this.tsWriter.Close();
            }
        }

        public override IDictionary<string, IEnumerable<Channel>> GetChannelFileMap()
        {
            var items = base.GetChannelFileMap();
            if ((this.Settings as TextDataWriterSettings).WriteTimestamps)
            {
                items.Add(this.FormatFilePath("{0}_ts.txt"), this.Pins.Values.Where(mwp => mwp.Channel != null).Select(mwp => mwp.Channel));
            }
            return items;
        }


        protected byte[] stupidByteBuffer;
        protected ActionBlock<ChannelFrame> GetActionBlock()
        {
            return new ActionBlock<ChannelFrame>(frame =>
            {
                if (!this.IsRecording) { return; }

                this.writer.WriteLine(string.Join("\t", frame.FrameData.OfType<object>().Select(i => string.Format("{0}", i))));
                if(this.tsWriter != null)
                {
                    this.tsWriter.Write(frame.Metadata.AbsoluteTime);
                }
                this.Performance.Increment();
            });
        }
    }
}
