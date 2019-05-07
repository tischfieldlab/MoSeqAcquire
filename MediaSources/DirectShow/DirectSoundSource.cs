using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;
using MoSeqAcquire.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.DirectSound;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    //[KnownType(typeof(DirectShowConfigSnapshot))]
    [DisplayName("Direct Sound Source")]
    [SettingsImplementation(typeof(DirectSoundConfig))]
    public class DirectSoundSource : MediaSource, IAudioProvider
    {
        public DirectSoundSource() : base()
        {
            this.Name = "Direct Sound Device";
            
        }
        public AudioCaptureDevice AudioDevice { get; protected set; }
        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            var items = new List<Tuple<string, string>>();
            var devices = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            foreach (var device in devices)
            {
                items.Add(new Tuple<string, string>(device.Description, device.Guid.ToString()));
            }
            return items;
        }
        public override bool Initalize(string DeviceId)
        {
            this.DeviceId = DeviceId;
            var devices = new AudioDeviceCollection(AudioDeviceCategory.Capture);

            var targetDevice = devices.FirstOrDefault(di => di.Guid.ToString().Equals(DeviceId));

            if (targetDevice == null)
            {
                this.Status = "Disconnected";
                return false;
            }

            this.Name = targetDevice.Description;
            this.Status = "Initializing";
            this.AudioDevice = new AudioCaptureDevice(targetDevice);
            

            this.RegisterChannel(new DirectSoundChannel(this));
            this.BindConfig();
            this.IsInitialized = true;

            return true;
        }
        public override void Start()
        {
            this.AudioDevice.Start();
            
            base.Start();
        }
        public override void Stop()
        {
            if (!this.IsInitialized) { return; }
            base.Stop();
            this.AudioDevice.SignalToStop();
            this.AudioDevice.WaitForStop();
        }

        protected void BindConfig()
        {
            /*var cfg = this.Settings as DirectShowConfig;
            cfg.RegisterComplexProperty(nameof(cfg.ImageFormat), new VideoCapabilitiesProperty(this));
            cfg.RegisterComplexProperty(nameof(cfg.PowerlineFrequency), new PowerLineFrequencyProperty(this));


            cfg.RegisterComplexProperty(nameof(cfg.Brightness), new ProcAmpPropInfo(this, VideoProcAmpProperty.Brightness));
            cfg.RegisterComplexProperty(nameof(cfg.Contrast), new ProcAmpPropInfo(this, VideoProcAmpProperty.Contrast));
            cfg.RegisterComplexProperty(nameof(cfg.Hue), new ProcAmpPropInfo(this, VideoProcAmpProperty.Hue));
            cfg.RegisterComplexProperty(nameof(cfg.Saturation), new ProcAmpPropInfo(this, VideoProcAmpProperty.Saturation));
            cfg.RegisterComplexProperty(nameof(cfg.Sharpness), new ProcAmpPropInfo(this, VideoProcAmpProperty.Sharpness));
            cfg.RegisterComplexProperty(nameof(cfg.Gamma), new ProcAmpPropInfo(this, VideoProcAmpProperty.Gamma));
            cfg.RegisterComplexProperty(nameof(cfg.ColorEnable), new ProcAmpPropInfo(this, VideoProcAmpProperty.ColorEnable));
            cfg.RegisterComplexProperty(nameof(cfg.WhiteBalance), new ProcAmpPropInfo(this, VideoProcAmpProperty.WhiteBalance));
            cfg.RegisterComplexProperty(nameof(cfg.BacklightCompensation), new ProcAmpPropInfo(this, VideoProcAmpProperty.BacklightCompensation));
            cfg.RegisterComplexProperty(nameof(cfg.Gain), new ProcAmpPropInfo(this, VideoProcAmpProperty.Gain));
            cfg.RegisterComplexProperty(nameof(cfg.DigitalMultiplier), new ProcAmpPropInfo(this, VideoProcAmpProperty.DigitalMultiplier));
            cfg.RegisterComplexProperty(nameof(cfg.DigitalMultiplierLimit), new ProcAmpPropInfo(this, VideoProcAmpProperty.DigitalMultiplierLimit));
            cfg.RegisterComplexProperty(nameof(cfg.WhiteBalanceComponent), new ProcAmpPropInfo(this, VideoProcAmpProperty.WhiteBalanceComponent));
            

            cfg.RegisterComplexProperty(nameof(cfg.Exposure), new CameraControlPropInfo(this, CameraControlProperty.Exposure));
            cfg.RegisterComplexProperty(nameof(cfg.Focus), new CameraControlPropInfo(this, CameraControlProperty.Focus));
            cfg.RegisterComplexProperty(nameof(cfg.Iris), new CameraControlPropInfo(this, CameraControlProperty.Iris));
            cfg.RegisterComplexProperty(nameof(cfg.Pan), new CameraControlPropInfo(this, CameraControlProperty.Pan));
            cfg.RegisterComplexProperty(nameof(cfg.Roll), new CameraControlPropInfo(this, CameraControlProperty.Roll));
            cfg.RegisterComplexProperty(nameof(cfg.Tilt), new CameraControlPropInfo(this, CameraControlProperty.Tilt));
            cfg.RegisterComplexProperty(nameof(cfg.Zoom), new CameraControlPropInfo(this, CameraControlProperty.Zoom));*/
        }
    }
}
