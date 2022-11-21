using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    public class TriggerActionLifetimeEventArgs : EventArgs
    {
        public TriggerEvent Trigger;
    }

    public class TriggerActionFinishedEventArgs : TriggerActionLifetimeEventArgs
    {
        public string Output;
    }

    public class TriggerActionFaultedEventArgs : TriggerActionFinishedEventArgs
    {
        public Exception Exception;
    }




    public class TriggerEventLifetimeEventArgs : EventArgs
    {
        
    }

    public class TriggerEventFinishedEventArgs : TriggerEventLifetimeEventArgs
    {
        public string Output;
    }

    public class TriggerEventFaultedEventArgs : TriggerEventFinishedEventArgs
    {
        public Exception Exception;
    }
}
