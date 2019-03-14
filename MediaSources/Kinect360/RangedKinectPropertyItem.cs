using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    class RangedKinectPropertyItem : ComplexProperty
    {
        KinectManager source;
        string valueProperty;
        string minProperty;
        string maxProperty;
        string autoProperty;
        object currentValue;

        public RangedKinectPropertyItem(KinectManager Source, string valueProperty, string minProperty, string maxProperty, string autoProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
            this.minProperty = minProperty;
            this.maxProperty = maxProperty;
            this.autoProperty = autoProperty;
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
                this.currentValue = value;
                this.PushCurrentValue();
            }
        }
        public override bool IsAutomatic
        {
            get
            {
                return (bool)this.GetPropertyValue(this.source.Sensor, this.autoProperty);
            }
            set
            {
                this.SetPropertyValue(this.source.Sensor, this.autoProperty, value);
            }
        }

        public override void ResetValue()
        {
            throw new NotImplementedException();
        }

        protected override void PushCurrentValue()
        {
            this.SetPropertyValue(this.source.Sensor, this.valueProperty, this.currentValue);
        }
        protected override void ReadCurrentValue()
        {
            this.currentValue = this.GetPropertyValue(this.source.Sensor, this.valueProperty);
        }

        protected override PropertyCapability ReadCapability()
        {
            var capability = new RangedPropertyCapability()
            {
                IsSupported = true,
                Min = this.GetPropertyValue(this.source.Sensor, this.minProperty),
                Max = this.GetPropertyValue(this.source.Sensor, this.maxProperty),
            };
            if (this.autoProperty != null)
            {
                capability.AllowsAuto = true;
            }
            return capability;
        }
        protected object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
        protected void SetPropertyValue(object src, string propName, object value)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                SetPropertyValue(GetPropertyValue(src, temp[0]), temp[1], value);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                if (prop != null)
                {
                    prop.SetValue(src, value);
                }
            }
        }
    }
}
