using MoSeqAcquire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Triggers
{
    public abstract class TriggerEvent : Component
    {
        public string Name { get; }

        public TriggerEventConfig Config { get; protected set; }


        public abstract void Start();

        public abstract void Stop();

    }

}
