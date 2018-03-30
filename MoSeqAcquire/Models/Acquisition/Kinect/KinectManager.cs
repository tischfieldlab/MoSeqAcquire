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
            if (KinectSensor.KinectSensors.Count > 0)
            {
                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    this.Status = potentialSensor.Status.ToString();
                    if (potentialSensor.Status == KinectStatus.Connected)
                    {
                        this.Sensor = potentialSensor;
                        break;
                    }
                }
            }
            else
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
            this.Sensor.ColorStream.Disable();
            this.Sensor.DepthStream.Disable();
            this.FindChannel<KinectSoundChannel>().Enabled = false;
            base.Stop();
        }  

        public KinectSensor Sensor { get; set; }
    }
}
