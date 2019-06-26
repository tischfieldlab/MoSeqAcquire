using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition
{
    public struct ChannelFrame
    {
        public ChannelFrame(Array Data) : this(Data, new ChannelFrameMetadata())
        {
        }
        public ChannelFrame(Array Data, ChannelFrameMetadata Metadata)
        {
            this.FrameData = Data;
            this.Metadata = Metadata;
        }
        public Array FrameData { get; private set; }
        public Type DataType { get => this.FrameData.GetType().GetElementType(); }
        public ChannelFrameMetadata Metadata { get; private set; } 
    }
}
