using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;

namespace MoSeqAcquire.Models.Acquisition
{
    public class MediaSourceSpecification : ComponentSpecification
    {
        public MediaSourceSpecification(Type Type) : base(Type)
        {
        }
    }
}
