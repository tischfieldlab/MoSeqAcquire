using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    class PowerLineFrequencyProperty : ProcAmpPropInfo
    {
        public PowerLineFrequencyProperty(DirectShowSource Source) : base(Source, VideoProcAmpProperty.PowerlineFrequency)
        {
        }

        protected override PropertyCapability ReadCapability()
        {
            var baseCap = (RangedPropertyCapability)base.ReadCapability();
            return new ChoicesPropertyCapability()
            {
                Choices = new List<object>()
                {
                    new Tuple<string, int>("Disabled", 0),
                    new Tuple<string, int>("50 Hz", 1),
                    new Tuple<string, int>("60 Hz", 2)
                },
                DisplayPath = "Item1",
                ValuePath = "Item2",
                Default = baseCap.Default,
                AllowsAuto = baseCap.AllowsAuto,
                IsSupported = baseCap.IsSupported
            };
        }
    }
}
