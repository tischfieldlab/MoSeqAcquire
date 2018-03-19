using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectDepthChannel : KinectChannel
    {
        public KinectDepthChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Kinect Depth Channel";
            Kinect.Sensor.DepthFrameReady += this.Sensor_DepthFrameReady;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(short);
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
                    var meta = new ChannelFrameMetadata()
                    {
                        FrameId = imageFrame.FrameNumber,
                        Timestamp = imageFrame.Timestamp,
                        Width = imageFrame.Width,
                        Height = imageFrame.Height,
                        BytesPerPixel = imageFrame.BytesPerPixel,
                        PixelFormat = PixelFormats.Gray16
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
