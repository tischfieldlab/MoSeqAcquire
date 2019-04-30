using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderProduct
    {
        public string Name { get; set; }
        public IEnumerable<ChannelViewModel> Channels { get; set; }
    }
}
