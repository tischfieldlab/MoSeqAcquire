namespace MoSeqAcquire.Models.Acquisition.KinectXboxOne
{
    public abstract class KinectChannel : Channel
    {
        public KinectManager Kinect { get; protected set; }

        public KinectChannel(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }

        public abstract void Dispose();

        internal abstract void BindConfig();
    }
}