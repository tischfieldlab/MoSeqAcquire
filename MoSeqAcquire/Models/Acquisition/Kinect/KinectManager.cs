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
            
            this.Config = (KinectConfig)ConfigurationManager.GetSection("mediaConfig/" + this.Name);
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
            this.RegisterChannel(new KinectDepthChannel(this));
            this.RegisterChannel(new KinectColorChannel(this));
            return true;
        }


        public override void Start()
        {
            this.Sensor.ColorStream.Enable();
            this.Sensor.DepthStream.Enable();
            //this.Sensor.AudioSource.Start();
            this.Sensor.Start();
            
        }

        public override void Stop()
        {
            
        }  

        public KinectSensor Sensor { get; set; }
    }
}
