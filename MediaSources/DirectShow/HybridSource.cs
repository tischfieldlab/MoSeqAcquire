using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;
using MoSeqAcquire.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Accord.DirectSound;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    //[KnownType(typeof(DirectShowConfigSnapshot))]
    [DisplayName("Hybrid Direct Show / Direct Sound Device")]
    [SettingsImplementation(typeof(HybridConfig))]
    public class HybridSource : MediaSource, IVideoProvider, IAudioProvider
    {
        public HybridSource() : base()
        {
            this.Name = "Hybrid Direct Show / Direct Sound Device";
            
        }
        public ExVideoCaptureDevice VideoDevice { get; protected set; }
        public AudioCaptureDevice AudioDevice { get; protected set; }
        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            var items = new List<Tuple<string, string>>();
            foreach (var device in this.FindCorrelatedDevices())
            {
                items.Add(new Tuple<string, string>(device.Key, ToGuid(device.Key).ToString()));
            }
            return items;
        }
        protected static Guid ToGuid(string input)
        {
            var provider = new MD5CryptoServiceProvider();
            var inputBytes = Encoding.Default.GetBytes(input);
            var hashBytes = provider.ComputeHash(inputBytes);
            var hashGuid = new Guid(hashBytes);
            return hashGuid;
        }

        protected Dictionary<string, HashSet<string>> FindCorrelatedDevices()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var audioDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            var clusters = new Dictionary<string, HashSet<string>>();
            foreach (var vd in videoDevices)
            {
                foreach (var ad in audioDevices)
                {
                    if (ad.Description.Contains(vd.Name))
                    {
                        if (!clusters.ContainsKey(vd.Name))
                        {
                            clusters[vd.Name] = new HashSet<string>();
                        }
                        clusters[vd.Name].Add(vd.MonikerString);
                        clusters[vd.Name].Add(ad.Guid.ToString());
                    }
                } 
            }

            return clusters;
        }
        public override bool Initalize(string DeviceId)
        {
            this.DeviceId = DeviceId;

            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var audioDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            var devices = this.FindCorrelatedDevices();

            var targetDevice = devices.FirstOrDefault(di => ToGuid(di.Key).ToString().Equals(DeviceId));

            if (object.Equals(targetDevice, new KeyValuePair<string, HashSet<string>>()))
            {
                this.Status = "Disconnected";
                return false;
            }

            this.Name = targetDevice.Key;
            this.Status = "Initializing";

            this.VideoDevice = videoDevices.Where(vd => targetDevice.Value.Contains(vd.MonikerString))
                                           .Select(vd => new ExVideoCaptureDevice(vd.MonikerString))
                                           .First();
            
            this.AudioDevice = audioDevices.Where(ad => targetDevice.Value.Contains(ad.Guid.ToString()))
                                           .Select(ad => new AudioCaptureDevice(ad))
                                           .First();
            
            this.RegisterChannel(new DirectShowChannel(this));
            this.RegisterChannel(new DirectSoundChannel(this));
            this.BindConfig();
            this.IsInitialized = true;
 
            return true;
        }
        public override void Start()
        {
            this.VideoDevice.Start();
            this.AudioDevice.Start();
            base.Start();
        }
        public override void Stop()
        {
            if (!this.IsInitialized) { return; }
            base.Stop();
            this.VideoDevice.SignalToStop();
            this.VideoDevice.WaitForStop();

            this.AudioDevice.SignalToStop();
            this.AudioDevice.WaitForStop();
        }

        protected void BindConfig()
        {
            var cfg = this.Settings as HybridConfig;
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
            cfg.RegisterComplexProperty(nameof(cfg.Zoom), new CameraControlPropInfo(this, CameraControlProperty.Zoom));
        }
    }
}
