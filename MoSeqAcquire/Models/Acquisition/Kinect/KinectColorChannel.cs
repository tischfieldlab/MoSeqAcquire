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
    public class KinectColorChannel : KinectChannel
    {
        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Kinect Color Channel";
            Kinect.Sensor.ColorFrameReady += Sensor_ColorFrameReady;
            
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
