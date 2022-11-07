using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Triggers;

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
        protected Channel() {
            var blockOptions = new DataflowBlockOptions()
            {
                EnsureOrdered = true
            };
            this.Buffer = new BufferBlock<ChannelFrame>(blockOptions);
            this.Performance = new TotalFrameCounter();
            this.Performance.Start();
        }
        public MediaType MediaType { get; protected set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string FullName => this.DeviceName + " - " + this.Name;
        public virtual bool Enabled { get; set; }
        public BufferBlock<ChannelFrame> Buffer { get; protected set; }
        //public Type DataType { get; protected set; }
        public TotalFrameCounter Performance { get; protected set; }
        public abstract ChannelMetadata Metadata { get; }


        public delegate void FrameCapturedHandler(object sender, EventArgs e);
        public event FrameCapturedHandler FrameCaptured;

        protected void PostFrame(ChannelFrame Frame)
        {
            this.Buffer.Post(Frame);
            this.FrameCaptured?.Invoke(this, new EventArgs());
            this.Performance.Increment();
        }
    }
}