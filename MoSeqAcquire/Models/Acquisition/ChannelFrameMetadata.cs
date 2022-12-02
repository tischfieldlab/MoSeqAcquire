using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public class ChannelMetadata
    {
        public Type DataType { get; set; }
        public double TargetFramesPerSecond { get; set; }
    }


    public class ChannelFrameMetadata
    {
        public long FrameId { get; set; }
        public DateTime AbsoluteTime { get; set; }
        public virtual int TotalBytes { get; set; }
    }
}
