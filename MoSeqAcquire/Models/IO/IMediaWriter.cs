using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.Models.IO
{
    public delegate string DestinationBaseResponse();

    public interface IMediaWriter
    {
        event DestinationBaseResponse RequestDestinationBase;

        void Start();
        void Stop();
        void ConnectChannel(Channel Channel);
    }
}
