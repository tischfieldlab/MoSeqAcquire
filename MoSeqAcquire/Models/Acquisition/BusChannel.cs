using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition
{
    public class BusChannel
    {
        public BusChannel(MediaSource Source, Channel Channel)
        {
            this.Source = Source;
            this.Channel = Channel;
            var opts = new DataflowBlockOptions() { EnsureOrdered = true };
            this.Feed = new BroadcastBlock<ChannelFrame>(item => item, opts);
            Channel.Buffer.LinkTo(this.Feed, new DataflowLinkOptions() { PropagateCompletion = true });
        }
        public MediaSource Source { get; }
        public Channel Channel { get; }
        public BroadcastBlock<ChannelFrame> Feed { get; }

        public void Complete()
        {
            this.Feed.Complete();
        }
    }
}
