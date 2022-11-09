using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Recording
{
    [DisplayName("Before Recording Started")]
    public class BeforeRecordingStartedTrigger : TriggerEvent
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
    [DisplayName("After Recording Started")]
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
    [DisplayName("Before Recording Finished")]
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
    [DisplayName("After Recording Finished")]
    public class AfterRecordingFinishedTrigger : TriggerEvent
    {
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
