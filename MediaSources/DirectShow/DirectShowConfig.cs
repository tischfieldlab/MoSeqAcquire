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
}
