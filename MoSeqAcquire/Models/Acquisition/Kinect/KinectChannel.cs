using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectChannel<T> : Channel<T>
    {
        public KinectChannel(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }
        public KinectManager Kinect { get; protected set; }
    }
    
}
