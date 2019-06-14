using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay
{
    interface IVisualizationOverlay
    {
        string Name { get; }
        bool Enabled { get; set; }
    }
}
