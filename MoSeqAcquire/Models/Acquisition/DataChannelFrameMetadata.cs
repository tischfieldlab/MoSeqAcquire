using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition
{
    public class DataChannelFrameMetadata : ChannelFrameMetadata
    {
        public Type DataType { get; set; }
    }
}
