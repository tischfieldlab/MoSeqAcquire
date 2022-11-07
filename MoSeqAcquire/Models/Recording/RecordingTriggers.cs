using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Recording
{
    public class BeforeRecordingStartedTrigger : TriggerEvent {

        public BeforeRecordingStartedTrigger()
        {

        }
        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
    public class AfterRecordingStartedTrigger : TriggerEvent
    {
        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
    public class BeforeRecordingFinishedTrigger : TriggerEvent
    {
        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
    public class AfterRecordingFinishedTrigger : TriggerEvent
    {
        public AfterRecordingFinishedTrigger()
        {
            this.Name = "After Recording Finished";
        }
        public bool Aborted { get; set; }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
