using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public class PropertyCapability : IDefaultInfo, IAutomaticInfo, ISupportInfo
    {
        public object Default { get; set; }
        public bool AllowsAuto { get; set; }
        public bool IsSupported { get; set; }
    }

    public class RangedPropertyCapability : PropertyCapability, IRangeInfo
    {
        public object Min { get; set; }
        public object Max { get; set; }
        public object Step { get; set; }
    }

    public class ChoicesPropertyCapability : PropertyCapability, IChoicesProvider
    {
        public IEnumerable<object> Choices { get; set; }
        public string DisplayPath { get; set; }
        public string ValuePath { get; set; }
    }
}
