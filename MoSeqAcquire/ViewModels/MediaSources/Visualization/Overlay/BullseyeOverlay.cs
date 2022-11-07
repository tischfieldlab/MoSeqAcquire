using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Views.Controls.Visualization.Overlay;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay
{
    public class BullseyeOverlay : BaseViewModel, IVisualizationOverlay
    {
        public string Name => "Bullseye Overlay";
        public object Content => new BullseyeOverlayControl();

        protected bool isEnabled;
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => SetField(ref this.isEnabled, value);
        }
    }
}
