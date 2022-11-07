using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Views.Controls.Visualization.Overlay;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay
{
    public class RuleOfThirdsOverlay : BaseViewModel, IVisualizationOverlay
    {
        public string Name => "Rule Of Thirds Overlay";
        public object Content => new RuleOfThirdsOverlayControl();

        protected bool isEnabled;
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => SetField(ref this.isEnabled, value);
        }
    }
}
