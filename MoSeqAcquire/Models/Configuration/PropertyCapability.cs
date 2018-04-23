using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public class PropertyCapability : IRangeInfo, IDefaultInfo, IAutomaticInfo, ISupportInfo
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
