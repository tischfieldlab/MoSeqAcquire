using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition
{
    public interface IChannel { }
    public abstract class Channel<T> : IChannel
    {
        public Channel() {
            var blockoptions = new DataflowBlockOptions() {
                EnsureOrdered = true
            };
            this.Buffer = new BufferBlock<ChannelFrame<T>>(blockoptions);
        }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public ConfigurationSection Config { get; }
        public BufferBlock<ChannelFrame<T>> Buffer { get; protected set; }

        //protected abstract T PrepareFrame();
    }
}
