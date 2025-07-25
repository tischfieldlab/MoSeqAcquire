﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Data
{
    public interface IDataVisualizationPlugin : IVisualizationPlugin
    {
        void OnNewFrame(ChannelFrame data);
    }
}
