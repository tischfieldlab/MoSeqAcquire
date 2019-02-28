using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Acquisition.KinectXBOne
{
    public class KinectDepthChannel : KinectChannel
    {
        public KinectDepthChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Depth Channel";
            this.DeviceName = "Microsoft Kinect";
            Kinect.Sensor.DepthFrameReady += this.Sensor_DepthFrameReady;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(short);
        }
        public override ChannelMetadata Metadata
        {
            get
            {
                var conf = this.Kinect.Config as KinectConfig;
                switch (conf.DepthImageFormat)
                {
                    case DepthImageFormat.Resolution80x60Fps30:
                        return new VideoChannelMetadata()
                        {
                            Width = 80,
                            Height = 60,
                            FramesPerSecond = 30,
                            BytesPerPixel = 16,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case DepthImageFormat.Resolution320x240Fps30:
                        return new VideoChannelMetadata()
                        {
                            Width = 320,
                            Height = 240,
                            FramesPerSecond = 30,
                            BytesPerPixel = 16,
                            PixelFormat = PixelFormats.Gray16
                        };
                    case DepthImageFormat.Resolution640x480Fps30:
                    default:
                        return new VideoChannelMetadata()
                        {
                            Width = 640,
                            Height = 480,
                            FramesPerSecond = 30,
                            BytesPerPixel = 16,
                            PixelFormat = PixelFormats.Gray16
                        };
                }
            }
        }

        public DepthImageStream InnerStream { get { return Kinect.Sensor.DepthStream; } }
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

        private short[] _pixelData;
        private void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame imageFrame = e.OpenDepthImageFrame())
            {
                if (imageFrame != null)
                {
                    var meta = new VideoChannelFrameMetadata()
                    {
                        FrameId = imageFrame.FrameNumber,
                        Timestamp = imageFrame.Timestamp,
                        AbsoluteTime = PreciseDatetime.Now,
                        Width = imageFrame.Width,
                        Height = imageFrame.Height,
                        BytesPerPixel = imageFrame.BytesPerPixel,
                        PixelFormat = PixelFormats.Gray16,
                        TotalBytes = imageFrame.PixelDataLength * imageFrame.BytesPerPixel
                    };

                    if (this._pixelData == null || this._pixelData.Length != imageFrame.PixelDataLength)
                    {
                        this._pixelData = new short[imageFrame.PixelDataLength];
                    }

                    imageFrame.CopyPixelDataTo(this._pixelData);
                    this.Buffer.Post(new ChannelFrame(this._pixelData, meta));
                }
            }
        }
    }
}
