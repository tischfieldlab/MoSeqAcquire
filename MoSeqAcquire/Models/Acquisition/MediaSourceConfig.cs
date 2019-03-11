using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSourceConfig : BaseConfiguration
    {
        public abstract void ReadState();
    }
}
