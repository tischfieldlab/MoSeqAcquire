using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptingTriggers
{
    [SettingsImplementation(typeof(WriteToConsoleConfig))]
    public class WriteToConsole : TriggerAction
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
