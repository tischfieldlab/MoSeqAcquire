using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.IO
{
    public abstract class MediaWriter : IMediaWriter
    {
        public MediaWriter()
        {
            this.Settings = new RecorderSettings();
            this.Specification = new RecorderSpecification(this.GetType());
            this.Stats = new MediaWriterStats();
        }
        public event DestinationBaseResponse RequestDestinationBase;
        protected string RequestBaseDestination()
        {
            return this.RequestDestinationBase?.Invoke();
        }
        public string Name { get; set; }
        public RecorderSettings Settings { get; set; }

        public bool IsRecording { get; protected set; }
        public MediaWriterStats Stats { get; protected set; }
        public RecorderSpecification Specification { get; protected set; }
        

        public abstract void ConnectChannel(Channel Channel);
        public abstract void Start();

        public abstract void Stop();
    }

    /*public abstract class MediaWriter<TSink> : MediaWriter where TSink : MediaWriterSink
    {
        protected List<TSink> sinks;
        public MediaWriter()
        {
            this.sinks = new List<TSink>();
        }

        //public abstract void ConnectChannel(Channel Channel);
        public abstract IEnumerable<string> ListDestinations();
        public virtual void Start(string basePath)
        {
            foreach (var s in this.sinks)
            {
                s.IsRecording = true;
                s.Open();
            }
        }
        public virtual void Stop()
        {
            foreach (var s in this.sinks)
            {
                s.Close();
                s.IsRecording = false;
            }
        }

    }


    public abstract class MediaWriterSink
    {
        protected RecorderSettings settings;
        protected Channel channel;

        protected BufferBlock<ChannelFrame> back_buffer;
        protected ActionBlock<ChannelFrame> sink;

        public MediaWriterSink(RecorderSettings settings, Channel channel)
        {
            this.settings = settings;
            this.channel = channel;
        }
        
        public bool IsRecording { get; set; }
        protected abstract ActionBlock<ChannelFrame> GetActionBlock(Type type);
        protected void AttachSink(Channel Channel)
        {
            this.back_buffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true });
            this.sink = this.GetActionBlock(Channel.DataType);
            MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.back_buffer);
            this.back_buffer.LinkTo(this.sink, new DataflowLinkOptions() { PropagateCompletion = true });
        }
        public abstract void Close();
        public abstract void Open();
    }*/
}
