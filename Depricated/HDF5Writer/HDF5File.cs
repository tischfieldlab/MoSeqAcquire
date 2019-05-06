//using Hdf5DotNetTools;
//using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Recording
{
    /*class HDF5FileWriter : MediaWriter<HDF5ChannelSink>
    {
        protected long fileid;
        
        
        public HDF5FileWriter(string filename) : base()
        {
            this.fileid = Hdf5.CreateFile("testChunks.H5");
        }

        public override void ConnectChannel(Channel Channel)
        {
            //this.sinks.Add(new HDF5ChannelSink(this.fileid, "", Channel));
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
    public class HDF5ChannelSink : MediaWriterSink
    {
        protected long fileid;
        protected string dataset_name;
        protected dynamic dataset; //of type ChunkedDataset<T>

        public HDF5ChannelSink(RecorderSettings settings, Channel channel) : base(settings, channel)
        {
            //this.fileid = fileid;
            this.dataset_name = dataset;
            this.AttachSink(channel);
        }
        protected void Initialize(Type dataType, ulong[] chunksize)
        {
            var cdtype = typeof(ChunkedDataset<>).MakeGenericType(dataType);
            this.dataset = Activator.CreateInstance(cdtype, new object[] { this.dataset_name, this.fileid, chunksize });
        }

        protected override ActionBlock<ChannelFrame> GetActionBlock(Type type)
        {
            return new ActionBlock<ChannelFrame>(frame =>
            {
                if (this.dataset == null)
                {
                    this.Initialize(frame.DataType, new ulong[] { (ulong)frame.FrameData.Length });
                }
                this.dataset.AppendDataset(frame.FrameData);
            });
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }*/
}
