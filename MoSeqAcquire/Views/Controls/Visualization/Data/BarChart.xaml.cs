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
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class BarChart : UserControl
    {
        public BarChart()
        {
            InitializeComponent();
            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.SizeChanged += BarChart_SizeChanged;
        }

        private void BarChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.CalculateXScale();
        }

        private List<TextBlock> freqLabels = new List<TextBlock>();
        private void GenerateLabels()
        {
            var absMax = Math.Ceiling(Math.Max(Math.Abs(this.minValueSeen), Math.Abs(this.maxValueSeen)));
            for (double f = -absMax; f < absMax; f += absMax / 10)
            {
                if (f == 0)
                    continue; //Skip Zero Hz

                //var val = Math.Round(f * (this._sampleRate / this._fftSize / 2.0));
                TextBlock tb = this.freqLabels.FirstOrDefault(t => t.Tag.Equals(f));
                if (tb == null)
                {
                    tb = new TextBlock()
                    {
                        Tag = f,
                        Text = f.ToString("E2"),
                    };
                    //tb.RenderTransform = new RotateTransform(90);
                    this.freqLabels.Add(tb);
                    this.MainCanvas.Children.Add(tb);
                }
                tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //Canvas.SetBottom(tb, ((float)f / (float)this._fftSize) * this.ActualHeight - (tb.DesiredSize.Height / 2));
                Canvas.SetLeft(tb, 0);
                Canvas.SetBottom(tb, (this.ActualHeight / 2) - (f * yScale));

            }
        }

        private int bins;
        private double xScale;
        private float minValueSeen;
        private float maxValueSeen;
        private double yScale;
        private List<Rectangle> _bars = new List<Rectangle>();
        public void Update(float[] data)
        {
            

            if (data.Length != bins)
            {
                bins = data.Length;
                this.CalculateXScale();
            }

            this.CalculateYScale(data);

            if (this.ActualHeight == 0 && this.ActualWidth == 0)
                return;

            for (int i = 0; i < data.Length; i++)
            {
                if (this._bars.Count <= i)
                {
                    this._bars.Add(new Rectangle());
                    this.MainCanvas.Children.Add(this._bars[i]);
                }

                try
                {
                    this._bars[i].Height = Math.Abs(data[i] * yScale);
                    this._bars[i].Width = this.xScale;
                    Canvas.SetLeft(this._bars[i], this.xScale * i);

                    if (data[i] < 0)
                    {
                        Canvas.SetTop(this._bars[i], this.ActualHeight / 2);
                    }
                    else
                    {
                        Canvas.SetTop(this._bars[i], this.ActualHeight / 2 - this._bars[i].Height);
                    }
                }
                catch(ArgumentException)
                {
                    //nothing to do!
                }

            }

        }

        public void CalculateYScale(float[] data)
        {
            foreach (var t in data)
            {
                this.minValueSeen = Math.Min(this.minValueSeen, t);
                this.maxValueSeen = Math.Max(this.maxValueSeen, t);
            }

            this.yScale = this.ActualHeight / (Math.Max(Math.Abs(this.minValueSeen), Math.Abs(this.maxValueSeen)) * 2);
            GenerateLabels();
        }

        public void CalculateXScale()
        {
            this.xScale = this.ActualWidth / this.bins;
        }
    }
}
