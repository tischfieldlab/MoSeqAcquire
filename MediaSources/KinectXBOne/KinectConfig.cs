using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Configuration;
using Microsoft.Kinect;
using System.ComponentModel;

namespace MoSeqAcquire.Models.Acquisition.KinectXBOne
{
    public class KinectConfig : MediaSourceConfig
    {
        protected DepthFrameSource depthFrameSource;
        protected ColorFrameSource colorFrameSource;

        protected KinectManager Kinect { get; set; }
        protected KinectSensor Sensor { get; set; }


        public KinectConfig(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }

        public override void ReadState()
        {
            this.colorFrameSource = this.Sensor.ColorFrameSource;
            this.depthFrameSource = this.Sensor.DepthFrameSource;

        }

        /// <summary>
        /// Checks if the passed in type is in the a passed in range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to check range of. The range is inclusive on both 
        /// ends.</param>
        /// <param name="min">Minimum value in the range, inclusive.</param>
        /// <param name="max">Maximum value in the range, inclusive.</param>
        /// <returns></returns>
        protected bool CheckRange<T>(T value, T min, T max) where T : IComparable
        {
            if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
                return true;

            return false;
        }

        #region Kinect Depth Settings
        /// <summary>
        /// This will change the 
        /// </summary>
        [Category("Depth Camera Settings")]
        [Description("Sets the depth image format.")]
        public DepthFrameSource DepthFrameSource
        {
            get => this.depthFrameSource;
            set
            {
                if (value != null && depthFrameSource != value)
                {
                    SetField(ref depthFrameSource, value);
                }
            }
        }
        #endregion

    }

    class RangedKinectPropertyItem<T> : ComplexProperty
    {
        KinectManager source;
        string valueProperty;
        string minProperty;
        string maxProperty;
        string autoProperty;
        T currentValue;

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
                this.currentValue = (T)value;
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
            this.currentValue = (T)this.GetPropertyValue(this.source.Sensor, this.valueProperty);
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
        public object GetPropertyValue(object src, string propName)
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
        public void SetPropertyValue(object src, string propName, object value)
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
