using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition
{
    public enum MediaType
    {
        Audio,
        Video
    }

    public abstract class Channel
    {
        public Channel() {
            var blockoptions = new DataflowBlockOptions() {
                EnsureOrdered = true
            };
            this.Buffer = new BufferBlock<ChannelFrame>(blockoptions);
        }
        public MediaType MediaType { get; protected set; }
        public string Name { get; set; }
        public virtual bool Enabled { get; set; }
        public ConfigurationSection Config { get; }
        public BufferBlock<ChannelFrame> Buffer { get; protected set; }
        public Type DataType { get; protected set; }
        public ChannelMetadata Metadata { get; protected set; }

        //protected abstract T PrepareFrame();
    }

    public class ChannelMetadata
    {

    }

    public class VideoChannelMetadata : ChannelMetadata
    {
        public double FramesPerSecond { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerPixel { get; set; }
        public PixelFormat PixelFormat { get; set; }
    }
}