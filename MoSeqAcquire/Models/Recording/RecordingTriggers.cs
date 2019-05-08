using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Recording
{
    public class BeforeRecordingStartedTrigger : Trigger
    {
        public BeforeRecordingStartedTrigger()
        {
            this.Name = "Before Recording Started";
        }
    }

    public class AfterRecordingStartedTrigger : Trigger
    {
        public AfterRecordingStartedTrigger()
        {
            this.Name = "After Recording Started";
        }
    }

    public class BeforeRecordingFinishedTrigger : Trigger
    {
        public BeforeRecordingFinishedTrigger()
        {
            this.Name = "Before Recording Finished";
        }
    }
    public class AfterRecordingFinishedTrigger : Trigger
    {
        public AfterRecordingFinishedTrigger()
        {
            this.Name = "After Recording Finished";
        }
        public bool Aborted { get; set; }
    }
}
