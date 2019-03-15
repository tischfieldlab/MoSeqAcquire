using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    abstract class KinectPropertyItem : ComplexProperty
    {
        protected object source;
        protected string valueProperty;

        public override object Value
        {
            get
            {
                return this.GetPropertyValue(this.source, this.valueProperty);
            }
            set
            {
                this.SetPropertyValue(this.source, this.valueProperty, value);
            }
        }
        public override void ResetValue() { throw new NotImplementedException(); }
        protected override void PushCurrentValue() { throw new NotImplementedException(); }
        protected override void ReadCurrentValue() { throw new NotImplementedException(); }
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
