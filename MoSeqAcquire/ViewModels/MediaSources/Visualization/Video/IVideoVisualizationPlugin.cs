using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Video
{
    public interface IVideoVisualizationPlugin : IVisualizationPlugin
    {
        void OnNewFrame(ChannelFrame data);
    }
}
