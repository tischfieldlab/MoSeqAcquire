using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Views.Controls.Visualization.Data;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Data
{
    class BarGraphVisualization : IDataVisualizationPlugin
    {
        public string Name => "Bar Graph";

        private readonly BarChart _barChart = new BarChart();
        public object Content => this._barChart;

        public void OnNewFrame(ChannelFrame data)
        {
            this._barChart.Update(data.FrameData as float[]);
        }
    }
}
