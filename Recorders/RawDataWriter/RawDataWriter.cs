using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Recording.RawDataWriter
{
    [KnownType(typeof(RawDataWriterSettings))]
    [DisplayName("Raw Data Writer")]
    [SettingsImplementation(typeof(RawDataWriterSettings))]
    [SupportedChannelType(MediaType.Any, ChannelCapacity.Multiple)]
    public class RawDataWriter : MediaWriter
    {
        protected MediaWriterPin dataPin;

        protected FileStream file;
        protected GZipStream compressor;
        protected BinaryWriter writer;
        protected TimestampCoWriter tsWriter;

        public RawDataWriter() : base()
        {
            this.dataPin = new MediaWriterPin(MediaType.Any, ChannelCapacity.Multiple, this.GetActionBlock);
            this.RegisterPin(this.dataPin);
        }
        
        protected override string Ext
        {
            get
            {
                string ext = "";
                if(this.dataPin.Channel == null)
                {
                    return "###";
                }
                if (this.dataPin.Channel.DataType == typeof(short))
                {
                    ext = "short";
                }
                else if (this.dataPin.Channel.DataType == typeof(byte))
                {
                    ext = "byte";
                }
                else
                {
                    ext = "unknown";
                }
                if ((this.Settings as RawDataWriterSettings).EnableGZipCompression)
                {
                    ext += ".gz";
                }
                return ext;
            }
        }

        public override void Start()
        {
            var cfg = this.Settings as RawDataWriterSettings;

            this.file = File.Open(this.FilePath, FileMode.Create);
            if (cfg.EnableGZipCompression)
            {
                this.compressor = new GZipStream(this.file, CompressionMode.Compress);
                this.writer = new BinaryWriter(this.compressor);
            }
            else
            {
                this.writer = new BinaryWriter(this.file);
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
            if ((this.Settings as RawDataWriterSettings).WriteTimestamps)
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

                if (frame.DataType == typeof(short))
                {
                    if (this.stupidByteBuffer == null) { this.stupidByteBuffer = new byte[frame.Metadata.TotalBytes]; }
                    Buffer.BlockCopy(frame.FrameData, 0, this.stupidByteBuffer, 0, frame.Metadata.TotalBytes);
                    this.writer.Write(this.stupidByteBuffer);
                }
                else
                {
                    this.writer.Write(frame.FrameData as byte[]);
                }
                if(this.tsWriter != null)
                {
                    this.tsWriter.Write(frame.Metadata.AbsoluteTime);
                }
                this.Performance.Increment();
            });
        }
    }
}
