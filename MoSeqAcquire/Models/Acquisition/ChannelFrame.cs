using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition
{
    public class ChannelFrame
    {
        public ChannelFrame(Array Data) : this(Data, new ChannelFrameMetadata())
        {
        }
        public ChannelFrame(Array Data, ChannelFrameMetadata Metadata)
        {
            this.FrameData = Data;
            this.Metadata = Metadata;
        }
        public Array FrameData { get; protected set; }
        public Type DataType { get => this.FrameData.GetType().GetElementType(); }
        public ChannelFrameMetadata Metadata { get; protected set; } 

    }

    public class ChannelFrameMetadata
    {
        public int FrameId { get; set; }
        public long Timestamp { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }
        public PixelFormat PixelFormat { get; set; }

    }
}
