using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoSeqAcquire.Views.Controls.Visualization.Core;

namespace MoSeqAcquire.Views.Controls.Visualization.Data
{
    /// <summary>
    /// Interaction logic for LineChart.xaml
    /// </summary>
    public partial class LineChart : BaseChart
    {
        public LineChart() : base()
        {
            InitializeComponent();

            this.TargetCanvas = this.MainCanvas;
            this.Measure(_infinateSize);
            this.YAxis.LabelsGenerated += Axis_LabelsGenerated;
            this.XAxis.LabelsGenerated += Axis_LabelsGenerated;
        }

        private void Axis_LabelsGenerated(object sender, EventArgs e)
        {
            this.Baseline.X1 = 0;
            this.Baseline.X2 = this.ActualWidth;
            this.Baseline.Y1 = this.Baseline.Y2 = this.YAxis.GetDataPosition(0).Item2;
        }

        public void Update(float[] data)
        {
            if (this.ActualHeight == 0 && this.ActualWidth == 0)
                return;

            this.XAxis.UpdateExtents(data);
            this.YAxis.UpdateExtents(data);


            for (int i = 0; i < data.Length; i++)
            {
                var xpos = this.XAxis.GetDataPosition(i);
                var ypos = this.YAxis.GetDataPosition(data[i]);

                Point p = new Point(xpos.Item2, ypos.Item2);

                if (SpectrumPolyline.Points.Count <= i)
                {
                    SpectrumPolyline.Points.Add(p);
                }
                else
                {
                    SpectrumPolyline.Points[i] = p;
                }
            }
        }
    }
}
