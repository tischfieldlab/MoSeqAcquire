using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition
{
    public class VideoChannelFrameMetadata : ChannelFrameMetadata
    {
        public int FrameId { get; set; }
        public long Timestamp { get; set; }
        public DateTime AbsoluteTime { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public int Stride { get => this.Width * this.BytesPerPixel; }
    }
}
