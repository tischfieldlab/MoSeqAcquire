using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    public class TriggerLifetimeEventArgs : EventArgs
    {
        public Trigger Trigger;
    }

    public class TriggerFinishedEventArgs : TriggerLifetimeEventArgs
    {
        public string Output;
    }

    public class TriggerFaultedEventArgs : TriggerFinishedEventArgs
    {
        public Exception Exception;
    }
}
