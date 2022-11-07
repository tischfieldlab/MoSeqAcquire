using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Views.Controls.Visualization.Overlay;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay
{
    public class CrosshairOverlay : BaseViewModel, IVisualizationOverlay
    {
        public string Name => "Crosshair Overlay";
        public object Content => new CrosshairOverlayControl();

        protected bool isEnabled;
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => SetField(ref this.isEnabled, value);
        }
    }
}
