using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Kinect;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    public class KinectConfig : MediaSourceConfig
    {
        #region Kinect Settings
        [Category("Kinect Settings")]
        [DisplayName("Force Infrared Emitter Off")]
        [Description("Sets the state of the Infrared emitter.")]
        public bool ForceInfraredEmitterOff
        {
            get => this.GetField<bool>();
            set => this.SetField<bool>(value);
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
            get => this.GetField<bool>();
            set => this.SetField<bool>(value);
        }

        [Category("Audio Settings")]
        public BeamAngleMode BeamAngleMode
        {
            get => this.GetField<BeamAngleMode>();
            set => this.SetField<BeamAngleMode>(value);
        }

        [Category("Audio Settings")]
        public EchoCancellationMode EchoCancellationMode
        {
            get => this.GetField<EchoCancellationMode>();
            set => this.SetField<EchoCancellationMode>(value);
        }

        [Category("Audio Settings")]
        public int EchoCancellationSpeakerIndex
        {
            get => this.GetField<int>();
            set => this.SetField<int>(value);
        }

        [Category("Audio Settings")]
        public double ManualBeamAngle
        {
            get => this.GetField<double>();
            set => this.SetField<double>(value);
        }

        [Category("Audio Settings")]
        public bool NoiseSuppression
        {
            get => this.GetField<bool>();
            set => this.SetField<bool>(value);
        }
        #endregion
    }
}
