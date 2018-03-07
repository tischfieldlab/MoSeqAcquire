using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectDepthChannel : KinectChannel<short[]>
    {
        public KinectDepthChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Kinect Depth Channel";
            Kinect.Sensor.DepthFrameReady += this.Sensor_DepthFrameReady;
        }


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
                        BytesPerPixel = imageFrame.BytesPerPixel
                    };

                    var pixelData = new short[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(pixelData);
                    this.Buffer.Post(new ChannelFrame<short[]>(pixelData, meta));
                }
            }
        }
    }
}
