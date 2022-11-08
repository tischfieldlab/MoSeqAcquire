using MoSeqAcquire.Models.Utility;
using System.Windows.Media;
using System;
using Microsoft.Azure.Kinect.Sensor;

namespace MoSeqAcquire.Models.Acquisition.KinectAzure
{
    public class KinectDepthChannel : KinectChannel
    {

        public KinectDepthChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Depth Channel";
            this.DeviceName = Kinect.Name;
            this.MediaType = MediaType.Video;
            // this.DataType = typeof(short);
        }

        public override ChannelMetadata Metadata
        {
            get
            {
                switch (this.Kinect.Sensor.CurrentDepthMode)
                {
                    
                    case DepthMode.NFOV_2x2Binned:
                        return new VideoChannelMetadata()
                        {
                            Width = 320,
                            Height = 288,
                            FramesPerSecond = FPStoFPS(this.Kinect.DeviceConfiguration.CameraFPS),
                            BytesPerPixel = 2,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case DepthMode.NFOV_Unbinned:
                        return new VideoChannelMetadata()
                        {
                            Width = 640,
                            Height = 576,
                            FramesPerSecond = FPStoFPS(this.Kinect.DeviceConfiguration.CameraFPS),
                            BytesPerPixel = 2,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case DepthMode.PassiveIR:
                        return new VideoChannelMetadata()
                        {
                            Width = 1024,
                            Height = 1024,
                            FramesPerSecond = FPStoFPS(this.Kinect.DeviceConfiguration.CameraFPS),
                            BytesPerPixel = 2,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case DepthMode.WFOV_2x2Binned:
                        return new VideoChannelMetadata()
                        {
                            Width = 512,
                            Height = 512,
                            FramesPerSecond = FPStoFPS(this.Kinect.DeviceConfiguration.CameraFPS),
                            BytesPerPixel = 2,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case DepthMode.WFOV_Unbinned:
                        return new VideoChannelMetadata()
                        {
                            Width = 1024,
                            Height = 1024,
                            FramesPerSecond = FPStoFPS(this.Kinect.DeviceConfiguration.CameraFPS),
                            BytesPerPixel = 2,
                            PixelFormat = PixelFormats.Gray16
                        };
                    default:
                        return new VideoChannelMetadata();

                }
            }
        }

        public override void Dispose()
        {
            //this.depthFrameReader.Dispose();
        }

        

        private ushort[] _pixelData;
        internal override void RecieveFrame(Capture capture)
        {
            if (capture != null && this.Enabled)
            {
                var meta = new VideoChannelFrameMetadata()
                {
                    
                    //FrameId = capture.Depth.DeviceTimestamp,
                    AbsoluteTime = PreciseDatetime.Now,
                    Width = capture.Depth.WidthPixels,
                    Height = capture.Depth.HeightPixels,
                    BytesPerPixel = ImageFormatToBytesPerPixel(capture.Depth.Format),
                    PixelFormat = ImageFormatToPixelFormat(capture.Depth.Format),
                    TotalBytes = (int)capture.Depth.Size,
                    
                };
                var numPixels = meta.Width * meta.Height;

                if (this._pixelData == null || this._pixelData.Length != numPixels)
                {
                    this._pixelData = new ushort[numPixels];
                }

                // Transform the depth image to the colour capera perspective.
                var _transformation = this.Kinect.Calibration.CreateTransformation();
                var depthFrame = _transformation.DepthImageToColorCamera(capture.Depth);

                depthFrame.GetPixels<ushort>().ToArray().CopyTo(this._pixelData, 0);


                this.PostFrame(new ChannelFrame(this._pixelData, meta));
            }
        }

        internal override void BindConfig()
        {
            KinectConfig cfg = this.Kinect.Settings as KinectConfig;
        }

        
    }
}