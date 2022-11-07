using System;
using System.Linq;
using NAudio.Dsp;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization
{
    public interface IVisualizationPlugin
    {
        string Name { get; }
        object Content { get; }
    }
}
