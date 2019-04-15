using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Recording
{
    public class BeforeRecordingStartedTrigger : Trigger { }
    public class AfterRecordingStartedTrigger : Trigger { }
    public class BeforeRecordingFinishedTrigger : Trigger { }
    public class AfterRecordingFinishedTrigger : Trigger
    {
        public bool Aborted { get; set; }
    }
}
