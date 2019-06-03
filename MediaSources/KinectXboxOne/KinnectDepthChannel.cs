using Microsoft.Kinect;
using MoSeqAcquire.Models.Utility;
using System.Windows.Media;
using System;

namespace MoSeqAcquire.Models.Acquisition.KinectXboxOne
{
    public class KinectDepthChannel : KinectChannel
    {
        private DepthFrameReader depthFrameReader;

        public override bool Enabled
        {
            get => !this.depthFrameReader.IsPaused;
            set
            {
                this.depthFrameReader.IsPaused = !value;
               // this.Kinect.Config.ReadState();
            }
        }

        public KinectDepthChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Depth Channel";
            this.DeviceName = "Microsoft Kinect XBOX One";
            this.MediaType = MediaType.Video;
            this.DataType = typeof(short);

            this.depthFrameReader = this.Kinect.Sensor.DepthFrameSource.OpenReader();
            this.depthFrameReader.FrameArrived += DepthFrameReader_FrameArrivedHandler;
        }

        public override ChannelMetadata Metadata
        {
            get
            {
                return new VideoChannelMetadata()
                { 
                    Width = depthFrameReader.DepthFrameSource.FrameDescription.Width,
                    Height = depthFrameReader.DepthFrameSource.FrameDescription.Height,
                    TargetFramesPerSecond = 30,
                    BytesPerPixel = (int)depthFrameReader.DepthFrameSource.FrameDescription.BytesPerPixel,
                    PixelFormat = PixelFormats.Gray16
                };
            }
        }

        public override void Dispose()
        {
            this.depthFrameReader.Dispose();
        }

        private ushort[] _pixelData;
        private void DepthFrameReader_FrameArrivedHandler(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    var meta = new VideoChannelFrameMetadata()
                    {
                        // FrameId = depthFrame.
                        //Timestamp = depthFrame.RelativeTime.Ticks,
                        AbsoluteTime = PreciseDatetime.Now,
                        Width = depthFrame.FrameDescription.Width,
                        Height = depthFrame.FrameDescription.Height,
                        BytesPerPixel = (int)depthFrame.FrameDescription.BytesPerPixel,
                        PixelFormat = PixelFormats.Gray16,
                        TotalBytes = (int)(depthFrame.FrameDescription.BytesPerPixel *
                                          depthFrame.FrameDescription.LengthInPixels)
                    };

                    if (this._pixelData == null || this._pixelData.Length != depthFrame.FrameDescription.LengthInPixels)
                    {
                        this._pixelData = new ushort[depthFrame.FrameDescription.LengthInPixels];
                    }

                    depthFrame.CopyFrameDataToArray(this._pixelData);

                    ColorSpacePoint[] colorSpacePoints = new ColorSpacePoint[_pixelData.Length];
                    this.Kinect.Sensor.CoordinateMapper.MapDepthFrameToColorSpace(this._pixelData, colorSpacePoints);

                    this.PostFrame(new ChannelFrame(this._pixelData, meta));
                }
            }
        }

        internal override void BindConfig()
        {
            KinectConfig cfg = this.Kinect.Settings as KinectConfig;
        }
    }
}