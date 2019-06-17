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

        public override bool Initialize(string DeviceId)
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
            
            this.RegisterChannel(new KinectDepthChannel(this));
            this.RegisterChannel(new KinectColorChannel(this));
            this.RegisterChannel(new KinectSoundChannel(this));
            this.BindConfig();

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
            KinectConfig cfg = this.Settings as KinectConfig;

            //sensor level config
            cfg.RegisterComplexProperty(nameof(cfg.ForceInfraredEmitterOff), new SimpleKinectPropertyItem(this.Sensor, nameof(this.Sensor.ForceInfraredEmitterOff)));
            cfg.RegisterComplexProperty(nameof(cfg.ElevationAngle), new RangedKinectPropertyItem(this.Sensor, nameof(this.Sensor.ElevationAngle)));

            //let each channel bind config
            this.Channels.ForEach((c) => (c as KinectChannel).BindConfig());
        }
    }
}
