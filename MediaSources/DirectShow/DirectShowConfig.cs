using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectShowConfig : MediaSourceConfig
    {
        private VideoCapabilities imageFormat;

        private ProcAmpPropInfo brightness;
        private ProcAmpPropInfo contrast;
        private ProcAmpPropInfo hue;
        private ProcAmpPropInfo saturation;
        private ProcAmpPropInfo sharpness;
        private ProcAmpPropInfo gamma;
        private ProcAmpPropInfo colorEnable;
        private ProcAmpPropInfo whiteBalance;
        private ProcAmpPropInfo backlightCompensation;
        private ProcAmpPropInfo gain;
        private ProcAmpPropInfo digitalMultiplier;
        private ProcAmpPropInfo digitalMultiplierLimit;
        private ProcAmpPropInfo whiteBalanceComponent;
        private ProcAmpPropInfo powerlineFrequency;

        private CameraControlPropInfo exposure;
        private CameraControlPropInfo focus;
        private CameraControlPropInfo iris;
        private CameraControlPropInfo pan;
        private CameraControlPropInfo roll;
        private CameraControlPropInfo tilt;
        private CameraControlPropInfo zoom;


        public DirectShowConfig(DirectShowSource Source)
        {
            this.Source = Source;
            this.brightness = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Brightness);
            this.contrast = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Contrast);
            this.hue = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Hue);
            this.saturation = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Saturation);
            this.sharpness = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Sharpness);
            this.gamma = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Gamma);
            this.colorEnable = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.ColorEnable);
            this.whiteBalance = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.WhiteBalance);
            this.backlightCompensation = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.BacklightCompensation);
            this.gain = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.Gain);
            this.digitalMultiplier = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.DigitalMultiplier);
            this.digitalMultiplierLimit = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.DigitalMultiplierLimit);
            this.whiteBalanceComponent = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.WhiteBalanceComponent);
            this.powerlineFrequency = new PowerLineFrequencyProperty(this.Source);

            this.exposure = new CameraControlPropInfo(this.Source, CameraControlProperty.Exposure);
            this.focus = new CameraControlPropInfo(this.Source, CameraControlProperty.Focus);
            this.iris = new CameraControlPropInfo(this.Source, CameraControlProperty.Iris);
            this.pan = new CameraControlPropInfo(this.Source, CameraControlProperty.Pan);
            this.roll = new CameraControlPropInfo(this.Source, CameraControlProperty.Roll);
            this.tilt = new CameraControlPropInfo(this.Source, CameraControlProperty.Tilt);
            this.zoom = new CameraControlPropInfo(this.Source, CameraControlProperty.Zoom);
        }
        protected DirectShowSource Source { get; set; }
        public override void ReadState()
        {
            if(this.Source.Device.VideoResolution == null)
            {
                this.ImageFormat = this.Source.Device.VideoCapabilities[0];
            }
            int gain_val;
            VideoProcAmpFlags gain_flg;
            this.Source.Device.GetVideoProcAmpProperty(VideoProcAmpProperty.Gain, out gain_val, out gain_flg);
        }

        [DisplayName("Image Format")]
        [ChoicesMethod("ImageFormatChoices")]
        public VideoCapabilities ImageFormat
        {
            get => this.imageFormat;
            set => this.SetField(ref this.imageFormat, value, () => { this.Source.Device.VideoResolution = value; });
        }
        public IEnumerable<VideoCapabilities> ImageFormatChoices()
        {
            return this.Source.Device.VideoCapabilities;
        }

        public ComplexProperty Brightness { get => this.brightness; }
        public ComplexProperty Contrast { get => this.contrast; }
        public ComplexProperty Hue { get => this.hue; }
        public ComplexProperty Saturation { get => this.saturation; }
        public ComplexProperty Sharpness { get => this.sharpness; }
        public ComplexProperty Gamma { get => this.gamma; }
        public ComplexProperty ColorEnable { get => this.colorEnable; }
        public ComplexProperty WhiteBalance { get => this.whiteBalance; }
        public ComplexProperty BacklightCompensation { get => this.backlightCompensation; }
        public ComplexProperty Gain { get => this.gain; }
        public ComplexProperty DigitalMultiplier { get => this.digitalMultiplier; }
        public ComplexProperty DigitalMultiplierLimit { get => this.digitalMultiplierLimit; }
        public ComplexProperty WhiteBalanceComponent { get => this.whiteBalanceComponent; }
        public ComplexProperty PowerlineFrequency { get => this.powerlineFrequency; }


        public ComplexProperty Exposure { get => this.exposure; }
        public ComplexProperty Focus { get => this.focus; }
        public ComplexProperty Iris { get => this.iris; }
        public ComplexProperty Pan { get => this.pan; }
        public ComplexProperty Roll { get => this.roll; }
        public ComplexProperty Tilt { get => this.tilt; }
        public ComplexProperty Zoom { get => this.zoom; }

    }

    class ProcAmpPropInfo : ComplexProperty
    {
        protected DirectShowSource source;
        protected VideoProcAmpProperty property;

        protected int currentValue;
        protected VideoProcAmpFlags currentFlags;


        public ProcAmpPropInfo(DirectShowSource Source, VideoProcAmpProperty Property)
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
            this.source.Device.SetVideoProcAmpProperty(this.property, this.currentValue, this.currentFlags);
        }
        protected override void ReadCurrentValue()
        {
            
            this.source.Device.GetVideoProcAmpProperty(this.property, out this.currentValue, out this.currentFlags);
        }
        protected override PropertyCapability ReadCapability()
        {
            int min, max, step, dflt;
            VideoProcAmpFlags flgs;
            this.source.Device.GetVideoProcAmpRange(property, out min, out max, out step, out dflt, out flgs);
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
            int min, max, step, dflt;
            CameraControlFlags flgs;
            this.source.Device.GetCameraPropertyRange(property, out min, out max, out step, out dflt, out flgs);
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
