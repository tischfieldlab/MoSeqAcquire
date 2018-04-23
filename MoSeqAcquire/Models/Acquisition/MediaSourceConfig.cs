using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSourceConfig : BaseConfiguration
    {
        public abstract void ReadState();
    }

    public class PropertyCapability : IRangeInfo
    {
        public object Min { get; set; }
        public object Max { get; set; }
        public object Step { get; set; }
        public object Default { get; set; }
        //true => auto; false =>manual; null => none;
        public bool AllowsAuto { get; set; }
        public bool IsSupported { get; set; }
    }
}
