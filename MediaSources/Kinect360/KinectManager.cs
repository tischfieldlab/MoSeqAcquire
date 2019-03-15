using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Configuration;
using MoSeqAcquire.Models.Attributes;
using System.ComponentModel;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    //[KnownType(typeof(KinectConfigSnapshot))]
    /*[KnownType(typeof(ColorImageFormat))]
    [KnownType(typeof(DepthImageFormat))]
    [KnownType(typeof(PowerLineFrequency))]
    [KnownType(typeof(BacklightCompensationMode))]
    [KnownType(typeof(DepthRange))]*/
    [DisplayName("Kinect 360")]
    [SettingsImplementation(typeof(KinectConfig))]
    public class KinectManager : MediaSource
    {
        public KinectManager() : base()
        {
            this.Name = "Kinect360";
            this.BindConfig();
        }
        public KinectSensor Sensor { get; set; }

        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            var items = new List<Tuple<string, string>>();
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                items.Add(new Tuple<string, string>(potentialSensor.UniqueKinectId, potentialSensor.DeviceConnectionId));
            }
            return items;
        }

        public override bool Initalize(string DeviceId)
        {
            this.DeviceId = DeviceId;
            var deviceFound = false;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    if (potentialSensor.DeviceConnectionId == DeviceId)
                    {
                        deviceFound = true;
                        this.Status = potentialSensor.Status.ToString();
                        if (potentialSensor.Status == KinectStatus.Connected)
                        {
                            this.Sensor = potentialSensor;
                            break;
                        }
                    }
                }
            }
            if(!deviceFound)
            {
                this.Status = KinectStatus.Disconnected.ToString();
            }
            if (this.Sensor == null) { return false; }

            this.Config.ReadState();
            this.RegisterChannel(new KinectDepthChannel(this));
            this.RegisterChannel(new KinectColorChannel(this));
            this.RegisterChannel(new KinectSoundChannel(this));
            this.IsInitialized = true;
            return true;
        }

        public override void Start()
        {
            this.Sensor.Start();
            this.Sensor.ColorStream.Enable();
            this.Sensor.DepthStream.Enable();
            this.FindChannel<KinectSoundChannel>().Enabled = true;
            base.Start();
        }

        public override void Stop()
        {
            if(!this.IsInitialized) { return; }
            base.Stop();
            this.Sensor.ColorStream.Disable();
            this.Sensor.DepthStream.Disable();
            this.FindChannel<KinectSoundChannel>().Enabled = false;   
        }



        protected void BindConfig()
        {
            KinectConfig cfg = this.Config as KinectConfig;
            ColorImageStream cis = this.Sensor.ColorStream;
            ColorCameraSettings ccs = cis.CameraSettings;

            DepthImageStream dis = this.Sensor.DepthStream;

            //sensor level
            cfg.RegisterComplexProperty(nameof(cfg.ElevationAngle), new RangedKinectPropertyItem(this.Sensor, nameof(this.Sensor.ElevationAngle)));

            //color camera level
            cfg.RegisterComplexProperty(nameof(cfg.Brightness), new RangedKinectPropertyItem(ccs, nameof(ccs.Brightness)));
            cfg.RegisterComplexProperty(nameof(cfg.Contrast), new RangedKinectPropertyItem(ccs, nameof(ccs.Contrast)));
            cfg.RegisterComplexProperty(nameof(cfg.Saturation), new RangedKinectPropertyItem(ccs, nameof(ccs.Saturation)));
            cfg.RegisterComplexProperty(nameof(cfg.Sharpness), new RangedKinectPropertyItem(ccs, nameof(ccs.Sharpness)));
            cfg.RegisterComplexProperty(nameof(cfg.WhiteBalance), new RangedKinectPropertyItem(ccs, nameof(ccs.WhiteBalance), nameof(ccs.AutoWhiteBalance)));
            cfg.RegisterComplexProperty(nameof(cfg.ExposureTime), new RangedKinectPropertyItem(ccs, nameof(ccs.ExposureTime), nameof(ccs.AutoExposure)));
            cfg.RegisterComplexProperty(nameof(cfg.FrameInterval), new RangedKinectPropertyItem(ccs, nameof(ccs.FrameInterval)));
            cfg.RegisterComplexProperty(nameof(cfg.Gain), new RangedKinectPropertyItem(ccs, nameof(ccs.Gain)));
            cfg.RegisterComplexProperty(nameof(cfg.Gamma), new RangedKinectPropertyItem(ccs, nameof(ccs.Gamma)));
            cfg.RegisterComplexProperty(nameof(cfg.Hue), new RangedKinectPropertyItem(ccs, nameof(ccs.Hue)));
            cfg.RegisterComplexProperty(nameof(cfg.ColorImageFormat), new EnumKinectPropertyItem(cis, nameof(cis.Format)));
            cfg.RegisterComplexProperty(nameof(cfg.PowerLineFrequency), new EnumKinectPropertyItem(ccs, nameof(ccs.PowerLineFrequency)));
            cfg.RegisterComplexProperty(nameof(cfg.BacklightCompensationMode), new EnumKinectPropertyItem(ccs, nameof(ccs.BacklightCompensationMode)));

            //depth camera level
            cfg.RegisterComplexProperty(nameof(cfg.DepthImageFormat), new EnumKinectPropertyItem(dis, nameof(dis.Format)));
            cfg.RegisterComplexProperty(nameof(cfg.DepthRange), new EnumKinectPropertyItem(dis, nameof(dis.Range)));
            cfg.RegisterComplexProperty(nameof(cfg.ForceInfraredEmitterOff), new SimpleKinectPropertyItem(dis, nameof(dis.Range)));

            //audio device level
            cfg.RegisterComplexProperty(nameof(cfg.BeamAngleMode), new EnumKinectPropertyItem(dis, nameof(dis.Range)));
            cfg.RegisterComplexProperty(nameof(cfg.AutomaticGainControlEnabled), new SimpleKinectPropertyItem(dis, nameof(dis.Range)));
            cfg.RegisterComplexProperty(nameof(cfg.NoiseSuppression), new SimpleKinectPropertyItem(dis, nameof(dis.Range)));


        }
    }
}
