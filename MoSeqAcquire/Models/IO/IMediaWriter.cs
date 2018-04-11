using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.Models.Recording
{
    public delegate string DestinationBaseResponse();

    public interface IMediaWriter
    {
        event DestinationBaseResponse RequestDestinationBase;


        string Name { get; }
        bool IsRecording { get; }


        void Start();
        void Stop();
        void ConnectChannel(Channel Channel);
        IDictionary<string, IList<Channel>> GetChannelFileMap();
    }
}
