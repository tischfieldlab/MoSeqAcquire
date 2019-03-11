using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Configuration;
using MoSeqAcquire.Models.Attributes;
using System.ComponentModel;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    //[KnownType(typeof(KinectConfigSnapshot))]
    /*[KnownType(typeof(ColorImageFormat))]
    [KnownType(typeof(DepthImageFormat))]
    [KnownType(typeof(PowerLineFrequency))]
    [KnownType(typeof(BacklightCompensationMode))]
    [KnownType(typeof(DepthRange))]*/
    [DisplayName("Direct Show Source")]
    [SettingsImplementation(typeof(KinectConfig))]
    public class KinectManager : MediaSource
    {
        public KinectManager() : base()
        {
            this.Name = "Kinect";
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
    }
}
