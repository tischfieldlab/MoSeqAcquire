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

namespace MoSeqAcquire.Models.IO.RawDataWriter
{
    [KnownType(typeof(RawDataWriterSettings))]
    [DisplayName("Raw Data Writer")]
    [SettingsImplementation(typeof(RawDataWriterSettings))]
    public class RawDataWriter : MediaWriter
    {
        protected Channel channel;
        protected BufferBlock<ChannelFrame> back_buffer;
        protected ActionBlock<ChannelFrame> sink;

        protected FileStream file;
        protected GZipStream compressor;
        protected BinaryWriter writer;


        public override void ConnectChannel(Channel Channel)
        {
            if (this.channel != null)
            {
                throw new InvalidOperationException("Channel was already connected!");
            }
            this.channel = Channel;
            this.back_buffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true });
            this.sink = this.GetActionBlock(Channel.DataType);
            MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.back_buffer);
            this.back_buffer.LinkTo(this.sink, new DataflowLinkOptions() { PropagateCompletion = true });

        }
        public string FilePath
        {
            get => Path.Combine(this.RequestBaseDestination(), this.Name + "." + this.Ext);
        }
        public string Ext
        {
            get
            {
                string ext = "";
                if (this.channel.DataType == typeof(short))
                {
                    ext = "short";
                }
                else if (this.channel.DataType == typeof(byte))
                {
                    ext = "byte";
                }
                else
                {
                    ext = "unknown";
                }
                if ((this.Settings as RawDataWriterSettings).EnableGZipCompression)
                {
                    return ext + ".gz";
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
        protected ActionBlock<ChannelFrame> GetActionBlock(Type type)
        {
            if (type == typeof(short))
            {
                return new ActionBlock<ChannelFrame>(frame =>
                {
                    if (!this.IsRecording) { return; }
                    if (this.stupidByteBuffer == null) { this.stupidByteBuffer = new byte[frame.Metadata.TotalBytes]; }
                    Buffer.BlockCopy(frame.FrameData, 0, this.stupidByteBuffer, 0, frame.Metadata.TotalBytes);
                    this.writer.Write(this.stupidByteBuffer);
                    this.Stats.Increment();
                });
            }
            else if (type == typeof(byte))
            {
                return new ActionBlock<ChannelFrame>(frame => 
                {
                    if (!this.IsRecording) { return; }
                    this.writer.Write(frame.FrameData as byte[]);
                    this.Stats.Increment();
                });
            }
            return null;
        }
    }

    
}
