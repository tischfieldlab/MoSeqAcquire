using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using MoSeqAcquire.Models.Acquisition.KinectXBone;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Acquisition.KinectXBone
{
    public class KinectManager : MediaSource
    {
        public KinectSensor Sensor { get; set; }

        public KinectManager()
        {
            this.Name = "Kinect XBOX One";
            this.Config = new KinectConfig(this);
            this.IsInitialized = false;
        }

        /// <summary>
        /// Adds the connected Kinect Device to the list of available devices. 
        /// NOTE: In v2.0.0.0 of the Microsoft.Kinect.dll, only ONE device is 
        /// supported at a time.
        /// </summary>
        /// <returns>List of aviable Kinect Sensors</returns>
        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            var items = new List<Tuple<string, string>>();
            
            KinectSensor sensor = KinectSensor.GetDefault();
            if (sensor != null)
            {
                items.Add(new Tuple<string, string>(this.Name, sensor.UniqueKinectId));
            }

            return items;
        }

        /// <summary>
        /// Initializes the Kinect so that it can be ready to use.
        /// </summary>
        /// <param name="DeviceId">The string representing the ID of the device</param>
        /// <returns>True if the deivce is initialized, false if it cannot be.</returns>
        public override bool Initalize(string DeviceId)
        {
            this.DeviceId = DeviceId;
            var deviceFound = false;

            KinectSensor sensor = KinectSensor.GetDefault();
            if (sensor != null)
            {
                if (sensor.UniqueKinectId == DeviceId)
                {
                    deviceFound = true;
                    this.Status = "Connected";
                    this.Sensor = sensor;
                }
            }

            if (!deviceFound)
            {
                this.Status = "Disconnected";
            }

            if (this.Sensor == null) { return false; }

            this.Config.ReadState();
            this.RegisterChannel(new KinectDepthChannel(this));
            //this.RegisterChannel(new KinectColorChannel(this));
            //this.RegisterChannel(new KinectSoundChannel(this));
            return true;
        }

        /// <summary>
        /// Starts the sensor so that it can now be used to record.
        /// </summary>
        public override void Start()
        {
            if (this.IsInitialized) return;

            // Opening the sensor and all other sources for reading
            // TODO(jared): Maybe make this toggleable depending on what the user wants, to 
            // save some computations?

            // TODO(jared): Check on the values of IsActive and IsPaused once these OpenReader()
            // methods are called.
            this.Sensor.Open();
            this.FindChannel<KinectDepthChannel>().Enabled = true;
            this.IsInitialized = true;

            base.Start();
        }

        /// <summary>
        /// Stops the sensor, and closes it appropriately.
        /// </summary>
        public override void Stop()
        {
            if (!this.IsInitialized) return;

            this.FindChannel<KinectDepthChannel>().Dispose();
            this.Sensor.Close();

            base.Stop();
        }
    }
}
