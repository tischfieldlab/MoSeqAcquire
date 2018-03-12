using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition
{

    public abstract class Channel
    {
        public Channel() {
            var blockoptions = new DataflowBlockOptions() {
                EnsureOrdered = true
            };
            this.Buffer = new BufferBlock<ChannelFrame>(blockoptions);
        }
        //public Type BufferType { get => this.Buffer. }
        public string Name { get; set; }
        public virtual bool Enabled { get; set; }
        public ConfigurationSection Config { get; }
        public BufferBlock<ChannelFrame> Buffer { get; protected set; }

        //protected abstract T PrepareFrame();
    }
}