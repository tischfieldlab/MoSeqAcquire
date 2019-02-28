using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public abstract class DirectShowChannel : Channel
    {
        public DirectShowChannel(DirectShowSource Source)
        {
            this.Device = Source;
        }
        public DirectShowSource Device { get; protected set; }
    }
    
}
