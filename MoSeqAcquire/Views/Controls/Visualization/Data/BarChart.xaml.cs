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
            this.Measure(this._infinateSize);
            this.SizeChanged += BarChart_SizeChanged;
        }

        private void BarChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.CalculateXScale();
        }

        private List<TextBlock> _labels = new List<TextBlock>();
        private readonly Size _infinateSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        private void GenerateLabels()
        {
            var step = this.CalcFreqLabelInterval(5, this.maxValueSeen);
            for (double f = 0; f <= this.maxValueSeen; f += step)
            {
                this.RenderYLabel(f);
                if (f != 0 /*&& this._lastNegativeSeen > 0*/)
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
                this._labels.Add(tb);
                this.MainCanvas.Children.Add(tb);
            }
            tb.Measure(this._infinateSize);
            Canvas.SetLeft(tb, 0);
            Canvas.SetBottom(tb, this.ValueToYPos(value).Item2);
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
            if (this.ActualHeight == 0 && this.ActualWidth == 0)
                return;            

            if (data.Length != bins)
            {
                bins = data.Length;
                this.CalculateXScale();
            }

            this.CalculateYScale(data);



            if (this._bars.Count > data.Length)
                this.ResetBars();

            for (int i = 0; i < data.Length; i++)
            {
                if (this._bars.Count <= i)
                {
                    this._bars.Add(new Rectangle());
                    this.MainCanvas.Children.Add(this._bars[i]);
                }

                try
                {
                    var pos = this.ValueToYPos(data[i]);
                    this._bars[i].Height = pos.Item1;
                    this._bars[i].Width = this.xScale;
                    Canvas.SetLeft(this._bars[i], this.BinToXPos(i));
                    Canvas.SetTop(this._bars[i], pos.Item2);
                }
                catch(ArgumentException)
                {
                    //nothing to do!
                }
            }
        }

        private void ResetBars()
        {
            this._bars
                .ToList()
                .ForEach(b => {
                    this._bars.Remove(b);
                    this.MainCanvas.Children.Remove(b);
                });
        }

        private double BinToXPos(int bin)
        {
            return this.xScale * bin;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Height, and position Top</returns>
        private (double, double) ValueToYPos(double value)
        {
            double height = Math.Abs(value * yScale);
            if (value < 0)
            {
                return (height,  this.ActualHeight / 2);
            }
            else
            {
                return (height, this.ActualHeight / 2 - height);
            }
        }

        private readonly ConcurrentCircularBuffer<float> _rangeBuffer = new ConcurrentCircularBuffer<float>(100);
        private int _lastNegativeSeen = 0;
        public void CalculateYScale(float[] data)
        {
            float max = float.MinValue;
            var lastLastNegSeen = this._lastNegativeSeen;
            foreach (var t in data)
            {
                max = Math.Max(max, Math.Abs(t));
                if (t < 0)
                {
                    this._lastNegativeSeen = 100;
                }
                else
                {
                    if (this._lastNegativeSeen > 0)
                        this._lastNegativeSeen--;
                }
            }

            this._rangeBuffer.Put(max);

            var prevMax = this.maxValueSeen;
            this.maxValueSeen = float.MinValue;
            foreach(var m in this._rangeBuffer.Read())
            {
                this.maxValueSeen = Math.Max(m, this.maxValueSeen);
            }

            if(prevMax != this.maxValueSeen || lastLastNegSeen != this._lastNegativeSeen)
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
