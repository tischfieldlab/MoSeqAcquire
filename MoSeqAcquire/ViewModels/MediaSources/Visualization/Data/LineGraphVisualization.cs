using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Views.Controls.Visualization.Data;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Data
{
    class LineGraphVisualization : IDataVisualizationPlugin
    {
        public string Name => "Line Graph";

        private readonly LineChart _barChart = new LineChart();
        public object Content => this._barChart;

        public void OnNewFrame(ChannelFrame data)
        {
            this._barChart.Update(data.FrameData as float[]);
        }
    }
}
