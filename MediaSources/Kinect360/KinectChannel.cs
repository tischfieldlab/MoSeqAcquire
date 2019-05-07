using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    public abstract class KinectChannel : Channel
    {
        public KinectChannel(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }
        public KinectManager Kinect { get; protected set; }
        internal abstract void BindConfig();
    }
    
}
