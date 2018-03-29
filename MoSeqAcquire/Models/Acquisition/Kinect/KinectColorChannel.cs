using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectColorChannel : KinectChannel
    {
        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Kinect Color Channel";
            Kinect.Sensor.ColorFrameReady += Sensor_ColorFrameReady;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);
            this.Kinect.Config.PropertyChanged += (s,e) => this.RecomputeMetadata();
            this.RecomputeMetadata();
        }

        private void RecomputeMetadata()
        {
            var conf = this.Kinect.Config as KinectConfig;
            switch (conf.ColorImageFormat)
            {
                case ColorImageFormat.RawBayerResolution1280x960Fps12:
                    /*this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 1280,
                        Height = 960,
                        FramesPerSecond = 12,
                        BytesPerPixel = 8,
                        PixelFormat = PixelFormats.
                    };*/
                    throw new NotImplementedException();
                    //break;
                case ColorImageFormat.RawBayerResolution640x480Fps30:
                    this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 640,
                        Height = 480,
                        FramesPerSecond = 30,
                        BytesPerPixel = 16,
                        PixelFormat = PixelFormats.Gray16
                    };
                    throw new NotImplementedException();
                    //break;
                case ColorImageFormat.RawYuvResolution640x480Fps15:
                    this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 640,
                        Height = 480,
                        FramesPerSecond = 30,
                        BytesPerPixel = 16,
                        PixelFormat = PixelFormats.Gray16
                    };
                    throw new NotImplementedException();
                    //break;
                case ColorImageFormat.YuvResolution640x480Fps15:
                    this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 640,
                        Height = 480,
                        FramesPerSecond = 30,
                        BytesPerPixel = 16,
                        PixelFormat = PixelFormats.Bgr32
                    };
                    break;
                case ColorImageFormat.InfraredResolution640x480Fps30:
                    this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 640,
                        Height = 480,
                        FramesPerSecond = 30,
                        BytesPerPixel = 16,
                        PixelFormat = PixelFormats.Gray16
                    };
                    break;
                case ColorImageFormat.RgbResolution1280x960Fps12:
                    this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 1280,
                        Height = 960,
                        FramesPerSecond = 12,
                        BytesPerPixel = 32,
                        PixelFormat = PixelFormats.Bgr32
                    };
                    break;
                case ColorImageFormat.RgbResolution640x480Fps30:
                default:
                    this.Metadata = new VideoChannelMetadata()
                    {
                        Width = 640,
                        Height = 480,
                        FramesPerSecond = 30,
                        BytesPerPixel = 32,
                        PixelFormat = PixelFormats.Bgr32
                    };
                    break;

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
                this.Kinect.Config.ReadState();
            }
        }

        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    var meta = new ChannelFrameMetadata()
                    {
                        FrameId = imageFrame.FrameNumber,
                        Timestamp = imageFrame.Timestamp,
                        Width = imageFrame.Width,
                        Height = imageFrame.Height,
                        BytesPerPixel = imageFrame.BytesPerPixel,
                        PixelFormat = PixelFormats.Bgr32
                    };
                    if(imageFrame.Format == ColorImageFormat.InfraredResolution640x480Fps30)
                    {
                        meta.PixelFormat = PixelFormats.Gray16;
                    }
                    this.Buffer.Post(new ChannelFrame(imageFrame.GetRawPixelData(), meta));
                }
            }
        }
    }
}
