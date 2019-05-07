namespace MoSeqAcquire.Models.Configuration
{
    public interface IConfigSnapshotProvider
    {
        ConfigSnapshot GetSnapshot();
        void ApplySnapshot(ConfigSnapshot snapshot);
    }
}
