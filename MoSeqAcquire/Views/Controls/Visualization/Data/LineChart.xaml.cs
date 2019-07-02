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
            this.SizeChanged += BarChart_SizeChanged;
        }

        private void BarChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Baseline.X1 = 0;
            this.Baseline.X2 = this.ActualWidth;
            this.Baseline.Y1 = this.ActualHeight / 2;
            this.Baseline.Y2 = this.ActualHeight / 2;
        }

        ///private int bins;
        //private double xScale;
        //private float minValueSeen;
        //private float maxValueSeen;
        //private double yScale;
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
                //Point p = new Point((this.xScale * i) + (this.xScale / 2), 
                //                    (this.ActualHeight / 2) - (data[i] * yScale));

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
        /*
        public void CalculateYScale(float[] data)
        {
            foreach (var t in data)
            {
                this.minValueSeen = Math.Min(this.minValueSeen, t);
                this.maxValueSeen = Math.Max(this.maxValueSeen, t);
            }

            this.yScale = this.ActualHeight / (Math.Max(Math.Abs(this.minValueSeen), Math.Abs(this.maxValueSeen)) * 2);
        }

        public void CalculateXScale()
        {
            this.xScale = this.ActualWidth / this.bins;
        }*/
    }
}
