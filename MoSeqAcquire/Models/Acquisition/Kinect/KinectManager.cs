using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectManagement;
using Microsoft.Kinect;
using System.Configuration;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectManager : MediaSource
    {
        public KinectManager() : base()
        {
            this.Name = "Kinect";
            this.Config = new KinectConfig(this);
        }

        public override bool Initalize()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.Sensor = potentialSensor;
                    break;
                }
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
        }

        public override void Stop()
        {
            this.Sensor.ColorStream.Disable();
            this.Sensor.DepthStream.Disable();
            this.FindChannel<KinectSoundChannel>().Enabled = false;
        }  

        public KinectSensor Sensor { get; set; }
    }
}
