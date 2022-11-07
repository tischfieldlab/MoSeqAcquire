using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    public class TriggerLifetimeEventArgs : EventArgs
    {
        public TriggerEvent Trigger;
    }
    public class TriggerFaultedEventArgs : TriggerLifetimeEventArgs
    {
        public Exception Exception;
    }
}
