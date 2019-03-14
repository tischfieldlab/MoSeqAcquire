using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.KinectXBone
{
    public abstract class KinectChannel : Channel
    {
        public KinectManager Kinect { get; protected set; }

        public KinectChannel(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }

        public abstract void Dispose();
    }
}
