using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Kinect;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    public class KinectColorChannel : KinectChannel
    {
        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Color Channel";
            this.DeviceName = "Microsoft Kinect";
            Kinect.Sensor.ColorFrameReady += Sensor_ColorFrameReady;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);
        }

        public override ChannelMetadata Metadata
        {
            get
            {
                var conf = this.Kinect.Settings as KinectConfig;
                switch (conf.ColorImageFormat)
                {
                    case ColorImageFormat.RawBayerResolution1280x960Fps12:
                        throw new NotImplementedException();
                    case ColorImageFormat.RawBayerResolution640x480Fps30:
                        throw new NotImplementedException();
                    case ColorImageFormat.RawYuvResolution640x480Fps15:
                        throw new NotImplementedException();
                    case ColorImageFormat.YuvResolution640x480Fps15:
                        return new VideoChannelMetadata()
                        {
                            Width = 640,
                            Height = 480,
                            TargetFramesPerSecond = 30,
                            BytesPerPixel = 16,
                            PixelFormat = PixelFormats.Bgr32
                        };
                    case ColorImageFormat.InfraredResolution640x480Fps30:
                        return new VideoChannelMetadata()
                        {
                            Width = 640,
                            Height = 480,
                            TargetFramesPerSecond = 30,
                            BytesPerPixel = 16,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case ColorImageFormat.RgbResolution1280x960Fps12:
                        return new VideoChannelMetadata()
                        {
                            Width = 1280,
                            Height = 960,
                            TargetFramesPerSecond = 12,
                            BytesPerPixel = 32,
                            PixelFormat = PixelFormats.Bgr32
                        };
                    case ColorImageFormat.RgbResolution640x480Fps30:
                    default:
                        return new VideoChannelMetadata()
                        {
                            Width = 640,
                            Height = 480,
                            TargetFramesPerSecond = 30,
                            BytesPerPixel = 32,
                            PixelFormat = PixelFormats.Bgr32
                        };

                }
            }
        }

        public ColorImageStream InnerStream { get { return Kinect.Sensor.ColorStream; } }
        public override bool Enabled
        {
            get => this.InnerStream.IsEnabled;
            set
            {
                if (this.Enabled)
                {
                    this.InnerStream.Disable();
                }
                else
                {
                    this.InnerStream.Enable();
                }
            }
        }

        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    var meta = new VideoChannelFrameMetadata()
                    {
                        FrameId = imageFrame.FrameNumber,
                        //Timestamp = imageFrame.Timestamp,
                        AbsoluteTime = PreciseDatetime.Now,
                        Width = imageFrame.Width,
                        Height = imageFrame.Height,
                        BytesPerPixel = imageFrame.BytesPerPixel,
                        PixelFormat = PixelFormats.Bgr32,
                        TotalBytes = imageFrame.PixelDataLength * imageFrame.BytesPerPixel
                    };
                    if(imageFrame.Format == ColorImageFormat.InfraredResolution640x480Fps30)
                    {
                        meta.PixelFormat = PixelFormats.Gray16;
                    }
                    this.PostFrame(new ChannelFrame(imageFrame.GetRawPixelData(), meta));
                }
            }
        }

        internal override void BindConfig()
        {
            KinectConfig cfg = this.Kinect.Settings as KinectConfig;
            ColorImageStream cis = this.InnerStream;
            ColorCameraSettings ccs = cis.CameraSettings;

            cfg.RegisterComplexProperty(nameof(cfg.Brightness), new RangedKinectPropertyItem(ccs, nameof(ccs.Brightness)));
            cfg.RegisterComplexProperty(nameof(cfg.Contrast), new RangedKinectPropertyItem(ccs, nameof(ccs.Contrast)));
            cfg.RegisterComplexProperty(nameof(cfg.Saturation), new RangedKinectPropertyItem(ccs, nameof(ccs.Saturation)));
            cfg.RegisterComplexProperty(nameof(cfg.Sharpness), new RangedKinectPropertyItem(ccs, nameof(ccs.Sharpness)));
            cfg.RegisterComplexProperty(nameof(cfg.WhiteBalance), new RangedKinectPropertyItem(ccs, nameof(ccs.WhiteBalance), nameof(ccs.AutoWhiteBalance)));
            cfg.RegisterComplexProperty(nameof(cfg.ExposureTime), new RangedKinectPropertyItem(ccs, nameof(ccs.ExposureTime), nameof(ccs.AutoExposure)));
            cfg.RegisterComplexProperty(nameof(cfg.FrameInterval), new RangedKinectPropertyItem(ccs, nameof(ccs.FrameInterval)));
            cfg.RegisterComplexProperty(nameof(cfg.Gain), new RangedKinectPropertyItem(ccs, nameof(ccs.Gain)));
            cfg.RegisterComplexProperty(nameof(cfg.Gamma), new RangedKinectPropertyItem(ccs, nameof(ccs.Gamma)));
            cfg.RegisterComplexProperty(nameof(cfg.Hue), new RangedKinectPropertyItem(ccs, nameof(ccs.Hue)));
            cfg.RegisterComplexProperty(nameof(cfg.ColorImageFormat), new EnumKinectPropertyItem(cis, nameof(cis.Format)));
            cfg.RegisterComplexProperty(nameof(cfg.PowerLineFrequency), new EnumKinectPropertyItem(ccs, nameof(ccs.PowerLineFrequency)));
            cfg.RegisterComplexProperty(nameof(cfg.BacklightCompensationMode), new EnumKinectPropertyItem(ccs, nameof(ccs.BacklightCompensationMode)));
        }
    }
}
