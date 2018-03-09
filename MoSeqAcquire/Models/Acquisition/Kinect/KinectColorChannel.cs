using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectColorChannel : KinectChannel<byte>
    {
        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Kinect Color Channel";
            Kinect.Sensor.ColorFrameReady += Sensor_ColorFrameReady;
        }
        public ColorImageStream InnerStream { get { return Kinect.Sensor.ColorStream; } }

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
                        BytesPerPixel = imageFrame.BytesPerPixel
                    };

                    var pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(pixelData);
                    this.Buffer.Post(new ChannelFrame<byte>(pixelData, meta));
                }
            }
        }
    }
}
