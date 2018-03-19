using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.IO
{
    public abstract class MediaWriter
    {

        public abstract void ConnectChannel(BusChannel Channel, string Dest);
        public abstract void Start();
        public abstract void Stop();

    }
}
