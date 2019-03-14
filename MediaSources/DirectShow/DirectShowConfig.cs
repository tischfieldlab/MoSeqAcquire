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
        //private VideoCapabilities imageFormat;


        public DirectShowConfig()
        {
        }
        //protected DirectShowSource Source { get; set; }
        public override void ReadState()
        {
            
        }

        [DisplayName("Image Format")]
        public VideoCapabilities ImageFormat
        {
            get => this.GetField<VideoCapabilities>();
            set => this.SetField<VideoCapabilities>(value);
        }

        public int Brightness
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Contrast
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Hue
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Saturation
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Sharpness
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Gamma
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int ColorEnable
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int WhiteBalance
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int BacklightCompensation
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Gain
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int DigitalMultiplier
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int DigitalMultiplierLimit
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int WhiteBalanceComponent
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int PowerlineFrequency
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }


        public int Exposure
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Focus
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Iris
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Pan
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Roll
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Tilt
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        public int Zoom
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
    }


    class VideoCapabilitiesProperty : ComplexProperty
    {
        protected DirectShowSource source;
        public VideoCapabilitiesProperty(DirectShowSource Source)
        {
            this.source = Source;
        }
        public override object Value
        {
            get => this.source.Device.VideoResolution;
            set => this.source.Device.VideoResolution = (VideoCapabilities)value;
        }
        public override bool IsAutomatic {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void ResetValue()
        {
            throw new NotImplementedException();
        }

        

        protected override PropertyCapability ReadCapability()
        {
            return new ChoicesPropertyCapability()
            {
                IsSupported = true,
                AllowsAuto = false,
                Choices = this.source.Device.VideoCapabilities,
                Default = this.source.Device.VideoCapabilities[0]

            };
        }

        protected override void ReadCurrentValue()
        {
            throw new NotImplementedException();
        }
        protected override void PushCurrentValue()
        {
            throw new NotImplementedException();
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
    
}
