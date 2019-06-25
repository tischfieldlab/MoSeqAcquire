using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay
{
    public interface IVisualizationOverlay
    {
        string Name { get; }
        bool IsEnabled { get; set; }
        object Content { get; }
    }
}
