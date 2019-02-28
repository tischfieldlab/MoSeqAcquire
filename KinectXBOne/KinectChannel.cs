namespace MoSeqAcquire.Models.Acquisition.KinectXBOne
{
    public abstract class KinectChannel : Channel
    {
        public KinectChannel(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }
        public KinectManager Kinect { get; protected set; }
    }
}
