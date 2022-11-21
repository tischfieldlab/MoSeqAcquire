using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.ViewModels.MediaSources.Visualization.Overlay;
using MoSeqAcquire.ViewModels.MediaSources.Visualization.Video;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public class VideoChannelViewModel : ChannelViewModel
    {
        public VideoChannelViewModel(Channel channel) : base(channel)
        {
            this.RegisterViewPlugin(new ColorVideoVisualization());
            this.SetChannelViewCommand.Execute(this.AvailableViews.First());

            this.RegisterOverlayPlugin(new CrosshairOverlay());
            this.RegisterOverlayPlugin(new BullseyeOverlay());
            this.RegisterOverlayPlugin(new RuleOfThirdsOverlay());
        }
        public override void BindChannel()
        {
            MediaBus.Instance.Subscribe(
                bc => bc.Channel == this.channel,
                new ActionBlock<ChannelFrame>(frame =>
                {
                    var meta = frame.Metadata as VideoChannelFrameMetadata;
                    this.sizeHelper.Ratio = (double)meta.Height / (double)meta.Width;
                    (this.SelectedView.VisualizationPlugin as IVideoVisualizationPlugin).OnNewFrame(frame);
                    this.Performance.Increment();
                }, 
                new ExecutionDataflowBlockOptions() { TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext() }
            ));
        }
    }
}
