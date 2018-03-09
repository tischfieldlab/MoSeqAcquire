using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public class ChannelFrame<T>
    {
        public ChannelFrame(T[] Data) : this(Data, new ChannelFrameMetadata())
        {
        }
        public ChannelFrame(T[] Data, ChannelFrameMetadata Metadata)
        {
            this.FrameData = Data;
            this.Metadata = Metadata;
        }
        public T[] FrameData { get; protected set; }
        public ChannelFrameMetadata Metadata { get; protected set; } 

    }

    public class ChannelFrameMetadata
    {
        public int FrameId { get; set; }
        public long Timestamp { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }

    }
}
