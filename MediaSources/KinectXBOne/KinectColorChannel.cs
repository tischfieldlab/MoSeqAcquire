using Microsoft.Kinect;
using MoSeqAcquire.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition.KinectXBone
{
    public class KinectColorChannel : KinectChannel
    {
        private ColorFrameReader colorFrameReader;

        public override bool Enabled
        {
            get => !this.colorFrameReader.IsPaused;
            set
            {
                this.colorFrameReader.IsPaused = !value;
                this.Kinect.Config.ReadState();
            }
        }

        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Color Channel";
            this.DeviceName = "Microsoft Kinect XBOX One";
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);

            this.colorFrameReader = this.Kinect.Sensor.ColorFrameSource.OpenReader();
            this.colorFrameReader.FrameArrived += ColorFrameReader_FrameArrivedHandler;
        }

        public override ChannelMetadata Metadata
        {
            get
            {
                var conf = this.Kinect.Config as KinectConfig;

                return new VideoChannelMetadata()
                {
                    Width = colorFrameReader.ColorFrameSource.FrameDescription.Width,
                    Height = colorFrameReader.ColorFrameSource.FrameDescription.Height,
                    FramesPerSecond = 30,
                    BytesPerPixel = (int)colorFrameReader.ColorFrameSource.FrameDescription.BytesPerPixel,
                    PixelFormat = PixelFormats.Bgr32
                };
            }
        }

        private byte[] _pixelData;
        private void ColorFrameReader_FrameArrivedHandler(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    var meta = new VideoChannelFrameMetadata()
                    {
                        Timestamp = colorFrame.RelativeTime.Ticks,
                        AbsoluteTime = PreciseDatetime.Now,
                        Width = colorFrame.FrameDescription.Width,
                        Height = colorFrame.FrameDescription.Height,
                        BytesPerPixel = (int)colorFrame.FrameDescription.BytesPerPixel,
                        PixelFormat = PixelFormats.Bgr32,
                        TotalBytes = (int)(colorFrame.FrameDescription.BytesPerPixel * 
                                           colorFrame.FrameDescription.LengthInPixels)
                    };

                    if (this._pixelData == null || this._pixelData.Length != colorFrame.FrameDescription.LengthInPixels)
                    {
                        this._pixelData = new byte[colorFrame.FrameDescription.LengthInPixels];
                    }

                    colorFrame.CopyRawFrameDataToArray(_pixelData);
                    this.Buffer.Post(new ChannelFrame(this._pixelData, meta));
                }
            }
        }

        public override void Dispose()
        {
            this.colorFrameReader.Dispose();
        }
    }
}
