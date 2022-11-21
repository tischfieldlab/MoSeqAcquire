using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Acquisition.KinectAzure
{

    [DisplayName("Kinect for Azure")]
    [SettingsImplementation(typeof(KinectConfig))]
    public class KinectManager : MediaSource
    {
        public Device Sensor { get; set; }

        public KinectManager() : base()
        {
            this.Name = "Kinect for Azure";
            //this.Settings = new KinectConfig();
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

            var count = Device.GetInstalledCount();
            for (int i = 0; i < count; i++)
            {
                var sensor = Device.Open(i);
                items.Add(new Tuple<string, string>(this.Name, sensor.SerialNum));
            }

            return items;
        }

        /// <summary>
        /// Initializes the Kinect so that it can be ready to use.
        /// </summary>
        /// <param name="DeviceId">The string representing the ID of the device</param>
        /// <returns>True if the deivce is initialized, false if it cannot be.</returns>
        public override bool Initialize(string DeviceId)
        {
            this.DeviceId = DeviceId;
            var deviceFound = false;

            var count = Device.GetInstalledCount();
            for (int i = 0; i < count; i++)
            {
                var sensor = Device.Open(i);
                if (sensor.SerialNum == DeviceId)
                {
                    deviceFound = true;
                    this.Status = "Connected";
                    this.Sensor = sensor;
                    break;
                }
            }

            if (!deviceFound)
            {
                this.Status = "Disconnected";
            }

            if (this.Sensor == null) { return false; }

            //this.Settings.ReadState();

            this.RegisterChannel(this.DepthChannel = new KinectDepthChannel(this));
            this.RegisterChannel(this.ColorChannel = new KinectColorChannel(this));
            //this.RegisterChannel(new KinectSoundChannel(this));
            this.BindConfig();
            return true;
        }

        /// <summary>
        /// Starts the sensor so that it can now be used to record.
        /// </summary>
        public override void Start()
        {
            if (this.IsInitialized) return;

            this.DeviceConfiguration = this.GetDeviceConfiguration();
            this.Sensor.StartCameras(this.DeviceConfiguration);
            this.Calibration = this.Sensor.GetCalibration();
            this.DepthChannel.Enabled = true;
            this.ColorChannel.Enabled = true;
            this.IsInitialized = true;

            Task.Run(() => this.CameraCapture());

            base.Start();
        }

        /// <summary>
        /// Stops the sensor, and closes it appropriately.
        /// </summary>
        public override void Stop()
        {
            if (!this.IsInitialized) return;

            this.FindChannel<KinectColorChannel>().Dispose();
            this.FindChannel<KinectDepthChannel>().Dispose();
            this.Sensor.StopCameras();

            base.Stop();
        }

        public void BindConfig()
        {
            KinectConfig cfg = this.Settings as KinectConfig;

            this.Channels.ForEach((c) => (c as KinectChannel).BindConfig());
        }

        private KinectColorChannel ColorChannel { get; set; }
        private KinectDepthChannel DepthChannel { get; set; }
        internal Calibration Calibration { get; set; }
        internal DeviceConfiguration DeviceConfiguration { get; private set; }

        protected DeviceConfiguration GetDeviceConfiguration()
        {
            KinectConfig cfg = this.Settings as KinectConfig;
            return new DeviceConfiguration()
            {
                CameraFPS = cfg.CameraFPS,
                ColorFormat = cfg.ColorFormat,
                ColorResolution = cfg.ColorResolution,
                DepthDelayOffColor = TimeSpan.FromMilliseconds(cfg.DepthDelayOffColor),
                DepthMode = cfg.DepthMode,
                DisableStreamingIndicator = cfg.DisableStreamingIndicator,
                SuboridinateDelayOffMaster = TimeSpan.FromMilliseconds(cfg.SuboridinateDelayOffMaster),
                SynchronizedImagesOnly = cfg.SynchronizedImagesOnly,
                WiredSyncMode = cfg.WiredSyncMode,
            };
        }

        private void CameraCapture()
        {
            while (true)
            {
                try
                {
                    using (var capture = this.Sensor.GetCapture())
                    {
                        this.DepthChannel.RecieveFrame(capture);
                        this.ColorChannel.RecieveFrame(capture);
                    }
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }
    }
}