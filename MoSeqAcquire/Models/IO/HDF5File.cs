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
    class HDF5FileWriter
    {
        
        public HDF5FileWriter(string filename)
        {

        }

        public void ConnectChannel(IChannel Channel, string Dest)
        {

        }

        public static ActionBlock<ChannelFrame<T>> GetWriter<T>(string filename, ulong[] chunksize) where T : struct
        {
            var fileId = Hdf5.CreateFile("testChunks.H5");

            // create a dataset and append two more datasets to it
            using (var chunkedDset = new ChunkedDataset<T>("/test", fileId, chunksize))
            {
                return new ActionBlock<ChannelFrame<T>>(frame => chunkedDset.AppendDataset(frame.FrameData));
            };

        }
    }
}
