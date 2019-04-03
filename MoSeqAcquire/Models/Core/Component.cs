using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Core
{
    public abstract class Component
    {
        public ComponentSpecification Specification { get; protected set; }
        public BaseConfiguration Settings { get; protected set; }
    }
}
