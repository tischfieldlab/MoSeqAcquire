using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition
{
    public class VideoChannelMetadata : ChannelMetadata
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public int FramesPerSecond { get; set; }
    }

    public class VideoChannelFrameMetadata : ChannelFrameMetadata
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public int Stride => this.Width * this.BytesPerPixel;

        public override int TotalBytes
        {
            get => this.Width * this.Height * this.BytesPerPixel;
            set => throw new InvalidOperationException("TotalBytes is automatically computed.");
        }
    }
}
