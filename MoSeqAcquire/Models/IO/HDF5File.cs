using Hdf5DotNetTools;
using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.IO
{
    class HDF5FileWriter : MediaWriter
    {
        protected long fileid;
        protected List<HDF5ChannelSink> sinks;
        
        
        public HDF5FileWriter(string filename)
        {
            this.fileid = Hdf5.CreateFile("testChunks.H5");
            this.sinks = new List<HDF5ChannelSink>();
        }

        public override void ConnectChannel(Channel Channel)
        {
            this.sinks.Add(new HDF5ChannelSink(this.fileid, "", Channel));
        }

        public override IEnumerable<string> ListDestinations()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
    class HDF5ChannelSink
    {
        protected long fileid;
        protected string dataset_name;
        protected dynamic dataset; //of type ChunkedDataset<T>
        protected BufferBlock<ChannelFrame> back_buffer;
        protected ActionBlock<ChannelFrame> sink;

        public HDF5ChannelSink(long fileid, string dataset, Channel channel)
        {
            this.fileid = fileid;
            this.dataset_name = dataset;
            this.AttachSink(channel);
        }
        protected void Initialize(Type dataType, ulong[] chunksize)
        {
            var cdtype = typeof(ChunkedDataset<>).MakeGenericType(dataType);
            this.dataset = Activator.CreateInstance(cdtype, new object[] { this.dataset_name, this.fileid, chunksize });
        }
        protected void AttachSink(Channel Channel)
        {
            this.back_buffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true });
            this.sink = new ActionBlock<ChannelFrame>(frame =>
            {
                if (this.dataset == null)
                {
                    this.Initialize(frame.DataType, new ulong[] { (ulong)frame.FrameData.Length });
                }
                this.dataset.AppendDataset(frame.FrameData);
            });
            MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.back_buffer);
            this.back_buffer.LinkTo(this.sink, new DataflowLinkOptions() { PropagateCompletion = true });
        }
    }
}
