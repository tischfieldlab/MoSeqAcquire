using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class SynapseCaptureConfig : MediaSourceConfig
    {
        /*private VideoCapabilities imageFormat;

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
        private CameraControlPropInfo zoom;*/


        public SynapseCaptureConfig(SynapseCaptureSource Source)
        {
            this.Source = Source;
            /*
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
            this.zoom = new CameraControlPropInfo(this.Source, CameraControlProperty.Zoom);*/
        }
        protected SynapseCaptureSource Source { get; set; }
        public override void ReadState()
        {
            /*
            if(this.Source.Device.VideoResolution == null)
            {
                this.ImageFormat = this.Source.Device.VideoCapabilities[0];
            }
            int gain_val;
            VideoProcAmpFlags gain_flg;
            this.Source.Device.GetVideoProcAmpProperty(VideoProcAmpProperty.Gain, out gain_val, out gain_flg);
            */
        }
        /*
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
        */
    }


}
