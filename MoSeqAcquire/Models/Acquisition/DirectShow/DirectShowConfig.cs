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
            this.powerlineFrequency = new ProcAmpPropInfo(this.Source, VideoProcAmpProperty.PowerlineFrequency);
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

        //[RangeMethod("BrightnessRange")]
        public ComplexProperty Brightness
        {
            get => brightness;
        }
        /*public PropertyCapability BrightnessRange()
        {
            return this.brightness.Capability;
        }*/

        [RangeMethod("ContrastRange")]
        public int Contrast
        {
            get => (int)contrast.Value;
            set
            {
                this.contrast.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability ContrastRange()
        {
            return this.contrast.Capability;
        }

        [RangeMethod("HueRange")]
        public int Hue
        {
            get => (int)hue.Value;
            set
            {
                this.hue.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability HueRange()
        {
            return this.hue.Capability;
        }

        [RangeMethod("SaturationRange")]
        public int Saturation
        {
            get => (int)saturation.Value;
            set
            {
                this.saturation.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability SaturationRange()
        {
            return this.saturation.Capability;
        }

        [RangeMethod("SharpnessRange")]
        public int Sharpness
        {
            get => (int)sharpness.Value;
            set
            {
                this.sharpness.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability SharpnessRange()
        {
            return this.sharpness.Capability;
        }

        [RangeMethod("GammaRange")]
        public int Gamma
        {
            get => (int)gamma.Value;
            set
            {
                this.gamma.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability GammaRange()
        {
            return this.gamma.Capability;
        }

        [RangeMethod("ColorEnableRange")]
        public int ColorEnable
        {
            get => (int)colorEnable.Value;
            set
            {
                this.colorEnable.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability ColorEnableRange()
        {
            return this.colorEnable.Capability;
        }

        [RangeMethod("WhiteBalanceRange")]
        public int WhiteBalance
        {
            get => (int)whiteBalance.Value;
            set
            {
                this.whiteBalance.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability WhiteBalanceRange()
        {
            return this.whiteBalance.Capability;
        }

        [RangeMethod("BacklightCompensationRange")]
        public int BacklightCompensation
        {
            get => (int)backlightCompensation.Value;
            set
            {
                this.backlightCompensation.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability BacklightCompensationRange()
        {
            return this.backlightCompensation.Capability;
        }

        [RangeMethod("GainRange")]
        public int Gain
        {
            get => (int)gain.Value;
            set
            {
                this.gain.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability GainRange()
        {
            return this.gain.Capability;
        }

        [RangeMethod("DigitalMultiplierRange")]
        public int DigitalMultiplier
        {
            get => (int)digitalMultiplier.Value;
            set
            {
                this.digitalMultiplier.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability DigitalMultiplierRange()
        {
            return this.digitalMultiplier.Capability;
        }

        [RangeMethod("DigitalMultiplierLimitRange")]
        public int DigitalMultiplierLimit
        {
            get => (int)digitalMultiplierLimit.Value;
            set
            {
                this.digitalMultiplierLimit.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public PropertyCapability DigitalMultiplierLimitRange()
        {
            return this.digitalMultiplierLimit.Capability;
        }

        [RangeMethod("WhiteBalanceComponentRange")]
        [AutomaticProperty("IsWhiteBalanceComponentAutomatic")]
        public int WhiteBalanceComponent
        {
            get => (int)whiteBalanceComponent.Value;
            set
            {
                this.whiteBalanceComponent.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        public bool IsWhiteBalanceComponentAutomatic
        {
            get => this.whiteBalanceComponent.IsAutomatic;
            set => this.whiteBalanceComponent.IsAutomatic = value;
        }
        public PropertyCapability WhiteBalanceComponentRange()
        {
            return this.whiteBalanceComponent.Capability;
        }

        //[ChoicesMethod("PowerlineFrequencyChoices",DisplayPath ="Item1", ValuePath ="Item2" )]
        public int PowerlineFrequency
        {
            get => (int)powerlineFrequency.Value;
            set
            {
                this.powerlineFrequency.Value = value;
                this.NotifyPropertyChanged();
            }
        }
        /*public IEnumerable<Tuple<string, int>> PowerlineFrequencyChoices()
        {
            var pfc = this.powerlineFrequency.Capability;
            var choices = new List<Tuple<string, int>>();
            for (int i = (int)pfc.Min; i < (int)pfc.Max; i += (int)pfc.Step)
            {
                choices.Add(new Tuple<string, int>(i + " Hz", i));
            }
            return choices;
        }*/




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
                    this.currentFlags |= VideoProcAmpFlags.Auto;
                }
                else
                {
                    this.currentFlags &= ~VideoProcAmpFlags.Auto;
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
                IsSupported = (flgs.HasFlag(VideoProcAmpFlags.None) ? false : true)
            };
        }

        public override void ResetValue()
        {
            this.currentValue = (int)this.Default;
            this.PushCurrentValue();
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
                    this.currentFlags |= CameraControlFlags.Auto;
                }
                else
                {
                    this.currentFlags &= ~CameraControlFlags.Auto;
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
                IsSupported = (flgs.HasFlag(CameraControlFlags.None) ? false : true)
            };
        }

        public override void ResetValue()
        {
            this.currentValue = (int)this.Default;
            this.PushCurrentValue();
        }
    }
}
