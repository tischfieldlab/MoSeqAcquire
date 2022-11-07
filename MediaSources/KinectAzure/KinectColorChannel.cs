using Microsoft.Azure.Kinect.Sensor;
using MoSeqAcquire.Models.Utility;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition.KinectAzure
{
    public class KinectColorChannel : KinectChannel
    {

        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Color Channel";
            this.DeviceName = Kinect.Name;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);
        }

        public override ChannelMetadata Metadata
        {
            get
            {
                var meta = new VideoChannelMetadata()
                {
                    FramesPerSecond = FPStoFPS(this.Kinect.DeviceConfiguration.CameraFPS),
                    BytesPerPixel = ImageFormatToBytesPerPixel(this.Kinect.DeviceConfiguration.ColorFormat),
                    PixelFormat = ImageFormatToPixelFormat(this.Kinect.DeviceConfiguration.ColorFormat)
                };

                switch (this.Kinect.Sensor.CurrentColorResolution)
                {
                    case ColorResolution.R720p:
                        meta.Width = 1280;
                        meta.Height = 720;
                        break;
                    case ColorResolution.R1080p:
                        meta.Width = 1920;
                        meta.Height = 1080;
                        break;
                    case ColorResolution.R1440p:
                        meta.Width = 2560;
                        meta.Height = 1440;
                        break;
                    case ColorResolution.R1536p:
                        meta.Width = 2048;
                        meta.Height = 1536;
                        break;
                    case ColorResolution.R2160p:
                        meta.Width = 3840;
                        meta.Height = 2160;
                        break;
                    case ColorResolution.R3072p:
                        meta.Width = 4096;
                        meta.Height = 3072;
                        break;
                    case ColorResolution.Off:
                        meta.Width = 0;
                        meta.Height = 0;
                        break;
                }
                return meta;
            }
        }

        private byte[] _pixelData;
        internal override void RecieveFrame(Capture capture)
        {
            if (capture != null && this.Enabled)
            {
                var meta = new VideoChannelFrameMetadata()
                {
                    //Timestamp = colorFrame.RelativeTime.Ticks,
                    AbsoluteTime = PreciseDatetime.Now,
                    Width = capture.Color.WidthPixels,
                    Height = capture.Color.HeightPixels,
                    BytesPerPixel = ImageFormatToBytesPerPixel(capture.Color.Format),
                    PixelFormat = ImageFormatToPixelFormat(capture.Color.Format),
                    TotalBytes = (int)capture.Color.Size,
                };

                var numPixels = meta.Width * meta.Height;
                if (this._pixelData == null || this._pixelData.Length != meta.TotalBytes)
                {
                    this._pixelData = new byte[meta.TotalBytes];
                }

                capture.Color.GetPixels<BGRA>().ToArray().CopyTo(this._pixelData, 0);

                this.PostFrame(new ChannelFrame(_pixelData, meta));
            }
        }

        public override void Dispose()
        {
            //this.colorFrameReader.Dispose();
        }

        internal override void BindConfig()
        {
            KinectConfig cfg = this.Kinect.Settings as KinectConfig;
        }
    }
}