using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MoSeqAcquire.Models.Acquisition;
using MvvmValidation;

namespace MoSeqAcquire.Models.Recording
{
    public enum ChannelCapacity
    {
        Multiple = -1,
        None = 0,
        Single = 1,
    }

    public class MediaWriterPin
    {
        protected string name;
        protected Channel channel;
        protected BufferBlock<ChannelFrame> backBuffer;
        protected Action<ChannelFrame> sinkFactory;
        protected ActionBlock<ChannelFrame> sink;

        public MediaWriterPin(MediaType mediaType, ChannelCapacity Capacity, Action<ChannelFrame> Worker)
        {
            this.MediaType = mediaType;
            this.Capacity = Capacity;
            this.sinkFactory = Worker;
            this.name = this.MediaType.ToString() + " Pin";
        }
        public MediaWriterPin(string name, MediaType mediaType, ChannelCapacity Capacity, Action<ChannelFrame> Worker) : this(mediaType, Capacity, Worker)
        {
            this.name = name;
        }
        
        public string Name  => this.name;
        public bool IsEmpty => this.channel == null;
        public bool HasChannel => this.channel != null;
        public MediaType MediaType { get; protected set; }
        public ChannelCapacity Capacity { get; protected set; }
        public Channel Channel
        {
            get => this.channel;
            set => this.channel = value;
        }
        public void Connect()
        {
            this.backBuffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true, });
            this.sink = new ActionBlock<ChannelFrame>(this.sinkFactory);
            MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.backBuffer);
            this.backBuffer.LinkTo(this.sink, new DataflowLinkOptions() { PropagateCompletion = true });
        }
        public Task Disconnect()
        {
            this.backBuffer.Complete();
            return this.sink.Completion;
        }
    }
}
