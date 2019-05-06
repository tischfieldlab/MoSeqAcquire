using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.Models.Acquisition
{
    public enum MediaType
    {
        Any,
        Audio,
        Video,
        Data
    }

    public abstract class Channel
    {
        public Channel() {
            var blockoptions = new DataflowBlockOptions()
            {
                EnsureOrdered = true
            };
            this.Buffer = new BufferBlock<ChannelFrame>(blockoptions);
            this.Performance = new TotalFrameCounter();
            this.Performance.Start();
        }
        public MediaType MediaType { get; protected set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string FullName { get => this.DeviceName + " - " + this.Name; }
        public virtual bool Enabled { get; set; }
        public BufferBlock<ChannelFrame> Buffer { get; protected set; }
        public Type DataType { get; protected set; }
        public TotalFrameCounter Performance { get; protected set; }
        public abstract ChannelMetadata Metadata { get; }

        protected void PostFrame(ChannelFrame Frame)
        {
            this.Buffer.Post(Frame);
            this.Performance.Increment();
        }
    }

    public class ChannelMetadata
    {

    }

    public class VideoChannelMetadata : ChannelMetadata
    {
        public double TargetFramesPerSecond { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public int FramesPerSecond { get; set; }
    }
}