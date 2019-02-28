using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Kinect;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectConfig : MediaSourceConfig
    {
        protected bool forceInfraredEmitterOff;
        protected int elevationAngle;

        protected ColorImageFormat colorImageFormat;
        protected DepthImageFormat depthImageFormat;
        protected double brightness;
        protected double contrast;
        private double saturation;
        private double sharpness;
        private int whiteBalance;
        private double exposureTime;
        private double frameInterval;
        private double gain;
        private double gamma;
        private double hue;
        private PowerLineFrequency powerLineFrequency;
        private bool autoExposure;
        private bool autoWhiteBalance;
        private BacklightCompensationMode backlightCompensationMode;
        private DepthRange depthRange;

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

            this.colorImageFormat = this.Sensor.ColorStream.Format;
            this.depthImageFormat = this.Sensor.DepthStream.Format;

            //Color Camera level settings
            this.brightness = this.ColorCameraSettings.Brightness;
            this.contrast = this.ColorCameraSettings.Contrast;
            this.saturation = this.ColorCameraSettings.Saturation;
            this.sharpness = this.ColorCameraSettings.Sharpness;
            this.whiteBalance = this.ColorCameraSettings.WhiteBalance;
            this.exposureTime = this.ColorCameraSettings.ExposureTime;
            this.frameInterval = this.ColorCameraSettings.FrameInterval;
            this.gain = this.ColorCameraSettings.Gain;
            this.gamma = this.ColorCameraSettings.Gamma;
            this.hue = this.ColorCameraSettings.Hue;
            this.powerLineFrequency = this.ColorCameraSettings.PowerLineFrequency;
            this.autoExposure = this.ColorCameraSettings.AutoExposure;
            this.autoWhiteBalance = this.ColorCameraSettings.AutoWhiteBalance;
            this.backlightCompensationMode = this.ColorCameraSettings.BacklightCompensationMode;

            this.depthRange = this.Kinect.Sensor.DepthStream.Range;

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

        protected bool CheckRange<T>(T value, T min, T max) where T : IComparable
        {
            if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            {
                return true;
            }
            return false;
        }

        #region Kinect Settings
        [Category("Kinect Settings")]
        //[DefaultValue(false)]
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
        [ConfigurationProperty("elevationAngle", DefaultValue = 0, IsRequired = false)]
        //[IntegerValidator(MaxValue=this.Kinect.Sensor.MaxElevationAngle, MinValue=this.Kinect.Sensor.MinElevationAngle)]
        public int ElevationAngle
        {
            get => elevationAngle;
            set
            {
                if (elevationAngle != value && this.CheckRange(value, this.Sensor.MinElevationAngle, this.Sensor.MaxElevationAngle))
                {
                    //this.Sensor.ElevationAngle = value;
                    SetField(ref elevationAngle, value, () => { this.Sensor.ElevationAngle = value; });
                }
            }
        }
        #endregion

        #region Kinect Depth Camera Settings
        [Category("Depth Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the depth image format.")]
        public DepthImageFormat DepthImageFormat
        {
            get => depthImageFormat;
            set
            {
                if (depthImageFormat != value && DepthImageFormat.Undefined != value)
                {
                    //this.Sensor.DepthStream.Enable(value);
                    SetField(ref depthImageFormat, value, () => { this.Sensor.DepthStream.Enable(value); });
                }
            }
        }

        [Category("Depth Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the backlight compensation setting of the camera.")]
        public DepthRange DepthRange
        {
            get => depthRange;
            set
            {
                if (depthRange != value)
                {
                    this.Sensor.DepthStream.Range = value;
                    SetField(ref depthRange, value);
                }

            }
        }
        #endregion

        #region Kinect Color Camera settings
        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the brightness of the camera.")]
        public ColorImageFormat ColorImageFormat
        {
            get => colorImageFormat;
            set
            {
                if (colorImageFormat != value && ColorImageFormat.Undefined != value)
                {
                    //this.Sensor.ColorStream.Enable(value);
                    SetField(ref colorImageFormat, value, () => { this.Sensor.ColorStream.Enable(value); });
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the brightness of the camera.")]
        public double Brightness
        {
            get => brightness;
            set
            {
                if (brightness != value && this.CheckRange(value, this.ColorCameraSettings.MinBrightness, this.ColorCameraSettings.MaxBrightness))
                {
                    this.ColorCameraSettings.Brightness = value;
                    SetField(ref brightness, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the contrast of the camera.")]
        public double Contrast
        {
            get => contrast;
            set
            {
                if (contrast != value && this.CheckRange(value, this.ColorCameraSettings.MinContrast, this.ColorCameraSettings.MaxContrast))
                {
                    this.ColorCameraSettings.Contrast = value;
                    SetField(ref contrast, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the saturation of the camera.")]
        public double Saturation
        {
            get => saturation;
            set
            {
                if (saturation != value && this.CheckRange(value, this.ColorCameraSettings.MinSaturation, this.ColorCameraSettings.MaxSaturation))
                {
                    this.ColorCameraSettings.Saturation = value;
                    SetField(ref saturation, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the sharpness of the camera.")]
        public double Sharpness
        {
            get => sharpness;
            set
            {
                if (sharpness != value && this.CheckRange(value, this.ColorCameraSettings.MinSharpness, this.ColorCameraSettings.MaxSharpness))
                {
                    this.ColorCameraSettings.Sharpness = value;
                    SetField(ref sharpness, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the white balance of the camera.")]
        public int WhiteBalance
        {
            get => whiteBalance;
            set
            {
                if (whiteBalance != value && this.CheckRange(value, this.ColorCameraSettings.MinWhiteBalance, this.ColorCameraSettings.MaxWhiteBalance))
                {
                    this.ColorCameraSettings.WhiteBalance = value;
                    SetField(ref whiteBalance, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the exposure time of the camera.")]
        public double ExposureTime
        {
            get => exposureTime;
            set
            {
                if (exposureTime != value && this.CheckRange(value, this.ColorCameraSettings.MinExposureTime, this.ColorCameraSettings.MaxExposureTime))
                {
                    this.ColorCameraSettings.ExposureTime = value;
                    SetField(ref exposureTime, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the frame interval of the camera.")]
        public double FrameInterval
        {
            get => frameInterval;
            set
            {
                if (frameInterval != value && this.CheckRange(value, this.ColorCameraSettings.MinFrameInterval, this.ColorCameraSettings.MaxFrameInterval))
                {
                    this.ColorCameraSettings.FrameInterval = value;
                    SetField(ref frameInterval, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the gain of the camera.")]
        public double Gain
        {
            get => gain;
            set
            {
                if (gain != value && this.CheckRange(value, this.ColorCameraSettings.MinGain, this.ColorCameraSettings.MaxGain))
                {
                    this.ColorCameraSettings.Gain = value;
                    SetField(ref gain, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the gamma of the camera.")]
        public double Gamma
        {
            get => gamma;
            set
            {
                if (gamma != value && this.CheckRange(value, this.ColorCameraSettings.MinGamma, this.ColorCameraSettings.MaxGamma))
                {
                    this.ColorCameraSettings.Gamma = value;
                    SetField(ref gamma, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the hue of the camera.")]
        public double Hue
        {
            get => hue;
            set
            {
                if (hue != value && this.CheckRange(value, this.ColorCameraSettings.MinHue, this.ColorCameraSettings.MaxHue))
                {
                    this.ColorCameraSettings.Hue = value;
                    SetField(ref hue, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the Power Line Frequency of the camera.")]
        public PowerLineFrequency PowerLineFrequency
        {
            get => powerLineFrequency;
            set
            {
                if (powerLineFrequency != value)
                {
                    this.ColorCameraSettings.PowerLineFrequency = value;
                    SetField(ref powerLineFrequency, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the autoexposure setting of the camera.")]
        public bool AutoExposure
        {
            get => autoExposure;
            set
            {
                if (autoExposure != value)
                {
                    this.ColorCameraSettings.AutoExposure = value;
                    SetField(ref autoExposure, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the auto white exposure setting of the camera.")]
        public bool AutoWhiteBalance
        {
            get => autoWhiteBalance;
            set
            {
                if (autoWhiteBalance != value)
                {
                    this.ColorCameraSettings.AutoWhiteBalance = value;
                    SetField(ref autoWhiteBalance, value);
                }

            }
        }

        [Category("Color Camera Settings")]
        //[DefaultValue(0d)]
        [Description("Sets the backlight compensation setting of the camera.")]
        public BacklightCompensationMode BacklightCompensationMode
        {
            get => backlightCompensationMode;
            set
            {
                if (backlightCompensationMode != value)
                {
                    this.ColorCameraSettings.BacklightCompensationMode = value;
                    SetField(ref backlightCompensationMode, value);
                }

            }
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

    class RangedKinectPropertyItem<T> : ComplexProperty
    {
        KinectManager source;
        string valueProperty;
        string minProperty;
        string maxProperty;
        string autoProperty;
        T currentValue;

        public RangedKinectPropertyItem(KinectManager Source, string valueProperty, string minProperty, string maxProperty, string autoProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
            this.minProperty = minProperty;
            this.maxProperty = maxProperty;
            this.autoProperty = autoProperty;
        }

        public override object Value
        {
            get
            {
                this.ReadCurrentValue();
                return this.currentValue;
            }
            set
            {
                this.currentValue = (T)value;
                this.PushCurrentValue();
            }
        }
        public override bool IsAutomatic
        {
            get
            {
                return (bool)this.GetPropertyValue(this.source.Sensor, this.autoProperty);
            }
            set
            {
                this.SetPropertyValue(this.source.Sensor, this.autoProperty, value);
            }
        }

        public override void ResetValue()
        {
            throw new NotImplementedException();
        }

        protected override void PushCurrentValue()
        {
            this.SetPropertyValue(this.source.Sensor, this.valueProperty, this.currentValue);
        }
        protected override void ReadCurrentValue()
        {
            this.currentValue = (T)this.GetPropertyValue(this.source.Sensor, this.valueProperty);
        }

        protected override PropertyCapability ReadCapability()
        {
            var capability = new RangedPropertyCapability()
            {
                IsSupported = true,
                Min = this.GetPropertyValue(this.source.Sensor, this.minProperty),
                Max = this.GetPropertyValue(this.source.Sensor, this.maxProperty),
            };
            if(this.autoProperty != null)
            {
                capability.AllowsAuto = true;
            }
            return capability;
        }
        public object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
        public void SetPropertyValue(object src, string propName, object value)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                SetPropertyValue(GetPropertyValue(src, temp[0]), temp[1], value);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                if (prop != null)
                {
                    prop.SetValue(src, value);
                }
            }
        }

    }
}
