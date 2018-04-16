using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        protected Channel channel;
        protected BufferBlock<ChannelFrame> back_buffer;
        protected ActionBlock<ChannelFrame> sink;

        protected MediaWriterPin dataPin;

        protected FileStream file;
        protected GZipStream compressor;
        protected BinaryWriter writer;

        public RawDataWriter() : base()
        {
            this.dataPin = new MediaWriterPin(MediaType.Any, ChannelCapacity.Single, this.GetActionBlock());
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
            this.file = File.Open(this.FilePath, FileMode.Create);
            if ((this.Settings as RawDataWriterSettings).EnableGZipCompression)
            {
                this.compressor = new GZipStream(this.file, CompressionMode.Compress);
                this.writer = new BinaryWriter(this.compressor);
            }
            else
            {
                this.writer = new BinaryWriter(this.file);
            }
            this.IsRecording = true;
            this.Stats.Start();
        }

        public override void Stop()
        {
            this.Stats.Stop();
            this.sink.Complete();
            this.sink.Completion.Wait();
            this.IsRecording = false;
            this.writer.Close();
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
                this.Stats.Increment();
            });
        }
    }
}
