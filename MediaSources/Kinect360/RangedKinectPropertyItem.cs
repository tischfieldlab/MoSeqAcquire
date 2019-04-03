using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    class RangedKinectPropertyItem : KinectPropertyItem
    {
        readonly string minProperty;
        readonly string maxProperty;
        readonly string autoProperty;

        public RangedKinectPropertyItem(object Source, string valueProperty, string minProperty, string maxProperty, string autoProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
            this.minProperty = minProperty;
            this.maxProperty = maxProperty;
            this.autoProperty = autoProperty;
        }
        public RangedKinectPropertyItem(object Source, string valueProperty, string minProperty, string maxProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
            this.minProperty = minProperty;
            this.maxProperty = maxProperty;
        }
        public RangedKinectPropertyItem(object Source, string valueProperty, string autoProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
            this.minProperty = "Min" + valueProperty;
            this.maxProperty = "Max" + valueProperty;
            this.autoProperty = autoProperty;
        }
        public RangedKinectPropertyItem(object Source, string valueProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
            this.minProperty = "Min" + valueProperty;
            this.maxProperty = "Max" + valueProperty;
        }

        
        public override bool IsAutomatic
        {
            get
            {
                if (this.autoProperty == null) { return false; }
                return (bool)this.GetPropertyValue(this.source, this.autoProperty);
            }
            set
            {
                if (this.autoProperty == null) { return; }
                this.SetPropertyValue(this.source, this.autoProperty, value);
            }
        }
        protected override PropertyCapability ReadCapability()
        {
            var capability = new RangedPropertyCapability()
            {
                IsSupported = true,
                Min = this.GetPropertyValue(this.source, this.minProperty),
                Max = this.GetPropertyValue(this.source, this.maxProperty),
            };
            if (this.autoProperty != null)
            {
                capability.AllowsAuto = true;
            }
            return capability;
        }
    }
}
