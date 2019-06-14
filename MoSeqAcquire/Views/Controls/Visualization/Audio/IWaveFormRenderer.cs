using System;
using System.Linq;

namespace MoSeqAcquire.Views.MediaSources.Visualization
{
    public interface IWaveFormRenderer
    {
        void AddValue(float maxValue, float minValue);
    }
}
