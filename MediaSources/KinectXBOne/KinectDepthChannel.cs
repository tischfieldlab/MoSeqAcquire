using Microsoft.Kinect;
using MoSeqAcquire.Models.Utility;
using System.Windows.Media;
using System.Threading.Tasks.Dataflow;
using System;

namespace MoSeqAcquire.Models.Acquisition.KinectXBone
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
                this.Kinect.Config.ReadState();
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
                var conf = this.Kinect.Config as KinectConfig;

                return new VideoChannelMetadata()
                {
                    Width = conf.DepthFrameSource.FrameDescription.Width,
                    Height = conf.DepthFrameSource.FrameDescription.Height,
                    FramesPerSecond = 30,
                    BytesPerPixel = (int)conf.DepthFrameSource.FrameDescription.BytesPerPixel,
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
                       Timestamp = depthFrame.RelativeTime.Ticks,
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
                    this.Buffer.Post(new ChannelFrame(this._pixelData, meta));
                }
            }
        }
    }
}
