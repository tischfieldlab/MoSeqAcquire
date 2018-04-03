using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectShowChannel : Channel
    {
        public DirectShowChannel(DirectShowSource Source)
        {
            this.Device = Source;
        }
        public DirectShowSource Device { get; protected set; }
    }
    
}
