using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay
{
    class CrosshairOverlay : BaseViewModel, IVisualizationOverlay
    {
        public string Name => "Crosshair Overlay";

        protected bool enabled;
        public bool Enabled
        {
            get => this.enabled;
            set => SetField(ref this.enabled, value);
        }
    }
}
