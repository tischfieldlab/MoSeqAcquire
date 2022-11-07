using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Recording;

namespace AudioWriter
{
    public class AudioWriterSettings : RecorderSettings
    {
        public WriteToConsole()
        {
            this.Config = new WriteToConsoleConfig();
        }
        protected override Action<TriggerEvent> Action
        {
            get
            {
                return delegate (TriggerEvent trigger)
                {
                    var settings = this.Config as WriteToConsoleConfig;
                    Console.WriteLine($"Trigger {trigger.Name} {trigger.GetType().Name}");
                };
            }
        }
    }


    public class WriteToConsoleConfig : TriggerActionConfig
    {
        
    }

}
