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
        public MediaWriter()
        {
            this.Settings = new RecordingSettings();
        }
        public RecordingSettings Settings { get; protected set; }

        public abstract void ConnectChannel(Channel Channel);
        public abstract IEnumerable<string> ListDestinations();
        public abstract void Start();
        public abstract void Stop();

    }
}
