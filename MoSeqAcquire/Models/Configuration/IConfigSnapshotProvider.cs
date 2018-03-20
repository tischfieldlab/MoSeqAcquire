using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public interface IConfigSnapshotProvider
    {
        ConfigSnapshot GetSnapshot();
        void ApplySnapshot(ConfigSnapshot snapshot);
    }
}
