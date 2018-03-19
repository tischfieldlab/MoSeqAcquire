using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.Models.IO
{
    public class RawDataWriter : MediaWriter
    {

        protected List<RawDataSink> sinks;

        public RawDataWriter()
        {
            this.sinks = new List<RawDataSink>();
        }
        public override void ConnectChannel(BusChannel Channel, string Dest)
        {
            this.sinks.Add(new RawDataSink(Dest, Channel));
        }

        public override void Start()
        {
            foreach(var s in this.sinks)
            {
                s.IsRecording = true;
            }
        }

        public override void Stop()
        {
            foreach (var s in this.sinks)
            {
                s.IsRecording = false;
                s.Close();
            }
        }
    }

    public class RawDataSink
    {
        protected string filename;
        protected BusChannel channel;
        
        protected BufferBlock<ChannelFrame> back_buffer;
        protected ActionBlock<ChannelFrame> sink;
        protected FileStream file;
        protected BinaryWriter writer;

        public RawDataSink(string filename, BusChannel channel)
        {
            this.filename = filename;
            this.file = File.Open(this.filename, FileMode.Create);
            this.writer = new BinaryWriter(this.file);
            this.AttachSink(channel);
        }
        public bool IsRecording { get; set; }
        protected ActionBlock<ChannelFrame> GetActionBlock(Type type)
        {
            if(type == typeof(short))
            {
                return new ActionBlock<ChannelFrame>(frame =>
                {
                    if(!this.IsRecording) { return; }
                    var d = frame.FrameData as short[];
                    for (var i = 0; i < d.Length; i++)
                    {
                        this.writer.Write(d[i]);
                    }
                });
            }else if(type == typeof(byte))
            {
                return new ActionBlock<ChannelFrame>(frame => { if (!this.IsRecording) { return; }  this.writer.Write(frame.FrameData as byte[]); });
            }
            return null;
        }
        protected void AttachSink(BusChannel Channel)
        {
            this.back_buffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true });
            this.sink = this.GetActionBlock(Channel.Channel.DataType);
            Channel.Feed.LinkTo(this.back_buffer);
            this.back_buffer.LinkTo(this.sink, new DataflowLinkOptions() { PropagateCompletion = true });
        }
        public void Close()
        {
            this.back_buffer.Complete();
            this.writer.Close();
        }
    }
}
