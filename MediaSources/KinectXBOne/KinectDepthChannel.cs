using Microsoft.Kinect;
using MoSeqAcquire.Models.Acquisition.KinectXBOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.KinectXBone
{
    public class KinectDepthChannel : KinectChannel
    {
        public override ChannelMetadata Metadata => throw new NotImplementedException();

        public KinectDepthChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Depth Channel";
            this.DeviceName = "Microsoft Kinect XBOX One";
            this.MediaType = MediaType.Video;
            this.DataType = typeof(short);
            Kinect.depthFrameReader.FrameArrived += DepthFrameReader_FrameArrivedHandler;
        }

        private short[] _pixelData;
        private void DepthFrameReader_FrameArrivedHandler(object sender, DepthFrameArrivedEventArgs e)
        {
            //DepthFrame e.FrameReference.AcquireFrame()
        }

    }
}
