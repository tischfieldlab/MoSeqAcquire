using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Kinect;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    public class KinectConfig : MediaSourceConfig
    {
        protected bool forceInfraredEmitterOff;
        protected int elevationAngle;

        

        private bool automaticGainControlEnabled;
        private BeamAngleMode beamAngleMode;
        private EchoCancellationMode echoCancellationMode;
        private int echoCancellationSpeakerIndex;
        private double manualBeamAngle;
        private bool noiseSuppression;


        public KinectConfig(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }
        public override void ReadState()
        {
            //Sensor level settings
            //this.elevationAngle = this.Kinect.Sensor.ElevationAngle;
            this.forceInfraredEmitterOff = this.Sensor.ForceInfraredEmitterOff;


            //audio
            this.automaticGainControlEnabled = this.Kinect.Sensor.AudioSource.AutomaticGainControlEnabled;
            this.beamAngleMode = this.Kinect.Sensor.AudioSource.BeamAngleMode;
            this.echoCancellationMode = this.Kinect.Sensor.AudioSource.EchoCancellationMode;
            this.echoCancellationSpeakerIndex = this.Kinect.Sensor.AudioSource.EchoCancellationSpeakerIndex;
            this.manualBeamAngle = this.Kinect.Sensor.AudioSource.ManualBeamAngle;
            this.noiseSuppression = this.Kinect.Sensor.AudioSource.NoiseSuppression;

            this.NotifyPropertyChanged(null);
        }

        protected KinectManager Kinect { get; set; }
        protected KinectSensor Sensor { get => Kinect.Sensor; }
        protected ColorCameraSettings ColorCameraSettings { get => Sensor.ColorStream.CameraSettings; }

        

        #region Kinect Settings
        [Category("Kinect Settings")]
        [DisplayName("Force Infrared Emitter Off")]
        [Description("Sets the state of the Infrared emitter.")]
        public bool ForceInfraredEmitterOff
        {
            get => forceInfraredEmitterOff;
            set
            {
                if (forceInfraredEmitterOff != value)
                {
                    this.Sensor.ForceInfraredEmitterOff = value;
                    SetField(ref forceInfraredEmitterOff, value);
                }
            }
        }


        [Category("Kinect Settings")]
        [DisplayName("Elevation Angle")]
        public int ElevationAngle
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }
        #endregion

        #region Kinect Depth Camera Settings
        [Category("Depth Camera Settings")]
        [Description("Sets the depth image format.")]
        public DepthImageFormat DepthImageFormat
        {
            get => this.GetField<DepthImageFormat>();
            set => this.SetField<DepthImageFormat>(value);
        }

        [Category("Depth Camera Settings")]
        [Description("Sets the backlight compensation setting of the camera.")]
        public DepthRange DepthRange
        {
            get => this.GetField<DepthRange>();
            set => this.SetField<DepthRange>(value);
        }
        #endregion

        #region Kinect Color Camera settings
        [Category("Color Camera Settings")]
        [Description("Sets the brightness of the camera.")]
        public ColorImageFormat ColorImageFormat
        {
            get => this.GetField<ColorImageFormat>();
            set => this.SetField<ColorImageFormat>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the brightness of the camera.")]
        public double Brightness
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the contrast of the camera.")]
        public double Contrast
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the saturation of the camera.")]
        public double Saturation
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the sharpness of the camera.")]
        public double Sharpness
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the white balance of the camera.")]
        public int WhiteBalance
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the exposure time of the camera.")]
        public double ExposureTime
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the frame interval of the camera.")]
        public double FrameInterval
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the gain of the camera.")]
        public double Gain
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the gamma of the camera.")]
        public double Gamma
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the hue of the camera.")]
        public double Hue
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Color Camera Settings")]
        [Description("Sets the Power Line Frequency of the camera.")]
        public PowerLineFrequency PowerLineFrequency
        {
            get => this.GetField<PowerLineFrequency>();
            set => this.SetField<PowerLineFrequency>(value);
        }



        [Category("Color Camera Settings")]
        [Description("Sets the backlight compensation setting of the camera.")]
        public BacklightCompensationMode BacklightCompensationMode
        {
            get => this.GetField<BacklightCompensationMode>();
            set => this.SetField<BacklightCompensationMode>(value);
        }
        #endregion

        #region Kinect Audio Settings
        [Category("Audio Settings")]
        public bool AutomaticGainControlEnabled
        {
            get => automaticGainControlEnabled;
            set
            {
                if (automaticGainControlEnabled != value)
                {
                    this.Sensor.AudioSource.AutomaticGainControlEnabled = value;
                    SetField(ref automaticGainControlEnabled, value);
                }

            }
        }
        [Category("Audio Settings")]
        public BeamAngleMode BeamAngleMode
        {
            get => beamAngleMode;
            set
            {
                if (beamAngleMode != value)
                {
                    this.Sensor.AudioSource.BeamAngleMode = value;
                    SetField(ref beamAngleMode, value);
                }

            }
        }
        [Category("Audio Settings")]
        public EchoCancellationMode EchoCancellationMode
        {
            get => echoCancellationMode;
            set
            {
                if (echoCancellationMode != value)
                {
                    this.Sensor.AudioSource.EchoCancellationMode = value;
                    SetField(ref echoCancellationMode, value);
                }

            }
        }
        [Category("Audio Settings")]
        public int EchoCancellationSpeakerIndex
        {
            get => echoCancellationSpeakerIndex;
            set
            {
                if (echoCancellationSpeakerIndex != value)
                {
                    this.Sensor.AudioSource.EchoCancellationSpeakerIndex = value;
                    SetField(ref echoCancellationSpeakerIndex, value);
                }

            }
        }
        [Category("Audio Settings")]
        public double ManualBeamAngle
        {
            get => manualBeamAngle;
            set
            {
                if (manualBeamAngle != value)
                {
                    this.Sensor.AudioSource.ManualBeamAngle = value;
                    SetField(ref manualBeamAngle, value);
                }

            }
        }
        [Category("Audio Settings")]
        public bool NoiseSuppression
        {
            get => noiseSuppression;
            set
            {
                if (noiseSuppression != value)
                {
                    this.Sensor.AudioSource.NoiseSuppression = value;
                    SetField(ref noiseSuppression, value);
                }

            }
        }
        #endregion
    }
}
