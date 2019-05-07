using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    class ProcAmpPropInfo : ComplexProperty
    {
        protected IVideoProvider source;
        protected VideoProcAmpProperty property;

        protected int currentValue;
        protected VideoProcAmpFlags currentFlags;


        public ProcAmpPropInfo(IVideoProvider Source, VideoProcAmpProperty Property)
        {
            this.source = Source;
            this.property = Property;
        }
        public override object Value
        {
            get
            {
                this.ReadCurrentValue();
                return this.currentValue;
            }
            set
            {
                this.currentValue = Convert.ToInt32(value);
                this.PushCurrentValue();
            }
        }
        public override bool IsAutomatic
        {
            get
            {
                this.ReadCurrentValue();
                return this.currentFlags.HasFlag(VideoProcAmpFlags.Auto);
            }
            set
            {
                if (value)
                {
                    this.currentFlags = VideoProcAmpFlags.Auto;
                }
                else
                {
                    this.currentFlags = VideoProcAmpFlags.Manual;
                }
                this.PushCurrentValue();
            }
        }

        protected override void PushCurrentValue()
        {
            if (!this.IsSupported)
                return;
            this.source.VideoDevice.SetVideoProcAmpProperty(this.property, this.currentValue, this.currentFlags);
        }
        protected override void ReadCurrentValue()
        {
            if (!this.IsSupported)
                return;
            this.source.VideoDevice.GetVideoProcAmpProperty(this.property, out this.currentValue, out this.currentFlags);
        }
        protected override PropertyCapability ReadCapability()
        {
            this.source.VideoDevice.GetVideoProcAmpRange(property, 
                                                    out int min, 
                                                    out int max, 
                                                    out int step, 
                                                    out int dflt, 
                                                    out VideoProcAmpFlags flgs);
            return new RangedPropertyCapability()
            {
                Min = min,
                Max = max,
                Step = step,
                Default = dflt,
                AllowsAuto = (flgs.HasFlag(VideoProcAmpFlags.Auto) ? true : false),
                IsSupported = (flgs.Equals(VideoProcAmpFlags.None) ? false : true)
            };
        }

        public override void ResetValue()
        {
            this.currentValue = (int)this.Default;
            this.PushCurrentValue();
        }
    }
}
