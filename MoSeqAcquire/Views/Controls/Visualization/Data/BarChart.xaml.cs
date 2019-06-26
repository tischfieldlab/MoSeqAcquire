using MoSeqAcquire.Models.Utility;
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

        private List<TextBlock> _labels = new List<TextBlock>();
        private void GenerateLabels()
        {
            var step = this.CalcFreqLabelInterval(5, this.maxValueSeen);
            for (double f = 0; f <= this.maxValueSeen; f += step)
            {
                this.RenderYLabel(f);
                if (f != 0)
                    this.RenderYLabel(-f);
            }
        }
        private void RenderYLabel(double value)
        {
            TextBlock tb = this._labels.FirstOrDefault(t => t.Tag.Equals(value));
            if (tb == null)
            {
                tb = new TextBlock()
                {
                    Tag = value,
                    Text = value.ToString("F1"),
                };
                //tb.RenderTransform = new RotateTransform(90);
                this._labels.Add(tb);
                this.MainCanvas.Children.Add(tb);
            }
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //Canvas.SetBottom(tb, ((float)f / (float)this._fftSize) * this.ActualHeight - (tb.DesiredSize.Height / 2));
            Canvas.SetLeft(tb, 0);
            Canvas.SetBottom(tb, (this.ActualHeight / 2) + (value * yScale));
        }
        private void ResetLabels()
        {
            foreach(var l in this._labels)
            {
                this.MainCanvas.Children.Remove(l);
            }
            this._labels.Clear();
        }
        private double CalcFreqLabelInterval(int tickCount, double range)
        {
            double unroundedTickSize = range / (tickCount - 1);
            double x = Math.Ceiling(Math.Log10(unroundedTickSize) - 1);
            double pow10x = Math.Pow(10, x);
            double roundedTickRange = Math.Ceiling(unroundedTickSize / pow10x) * pow10x;
            return roundedTickRange;
        }

        private int bins;
        private double xScale;
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

        private ConcurrentCircularBuffer<float> _rangeBuffer = new ConcurrentCircularBuffer<float>(300);

        public void CalculateYScale(float[] data)
        {
            float max = float.MinValue;
            foreach (var t in data)
            {
                max = Math.Max(max, Math.Abs(t));
            }

            this._rangeBuffer.Put(max);

            var prevMax = this.maxValueSeen;
            this.maxValueSeen = float.MinValue;
            foreach(var m in this._rangeBuffer.Read())
            {
                this.maxValueSeen = Math.Max(m, this.maxValueSeen);
            }

            if(prevMax != this.maxValueSeen)
            {
                this.ResetLabels();
            }


            this.yScale = this.ActualHeight / (this.maxValueSeen * 2);
            GenerateLabels();
        }
        

        public void CalculateXScale()
        {
            this.xScale = this.ActualWidth / this.bins;
        }
    }
}
