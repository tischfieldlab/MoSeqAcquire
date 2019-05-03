using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    class CameraControlPropInfo : ComplexProperty
    {
        protected DirectShowSource source;
        protected CameraControlProperty property;

        protected int currentValue;
        protected CameraControlFlags currentFlags;


        public CameraControlPropInfo(DirectShowSource Source, CameraControlProperty Property)
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
                return this.currentFlags.HasFlag(CameraControlFlags.Auto);
            }
            set
            {
                if (value)
                {
                    this.currentFlags = CameraControlFlags.Auto;
                }
                else
                {
                    this.currentFlags = CameraControlFlags.Manual;
                }
                this.PushCurrentValue();
            }
        }

        protected override void PushCurrentValue()
        {
            this.source.Device.SetCameraProperty(this.property, this.currentValue, this.currentFlags);
        }
        protected override void ReadCurrentValue()
        {
            this.source.Device.GetCameraProperty(this.property, out this.currentValue, out this.currentFlags);
        }
        protected override PropertyCapability ReadCapability()
        {
            this.source.Device.GetCameraPropertyRange(property, 
                                                      out int min, 
                                                      out int max, 
                                                      out int step, 
                                                      out int dflt, 
                                                      out CameraControlFlags flgs);
            return new RangedPropertyCapability()
            {
                Min = min,
                Max = max,
                Step = step,
                Default = dflt,
                AllowsAuto = (flgs.HasFlag(CameraControlFlags.Auto) ? true : false),
                IsSupported = (flgs.Equals(CameraControlFlags.None) ? false : true)
            };
        }

        public override void ResetValue()
        {
            this.currentValue = (int)this.Default;
            this.PushCurrentValue();
        }
    }
}
